﻿using WabiSabiMonitor.ApplicationCore.Data;
using WabiSabiMonitor.ApplicationCore.Interfaces;
using WabiSabiMonitor.ApplicationCore.Utils.Logging;

namespace WabiSabiMonitor.ApplicationCore;

public class ApplicationCore
{
    private readonly Scraper _roundStatusScraper;
    private readonly IRoundDataReaderService _dataProcessor;
    private readonly IRpcServerController _rpcServerController;
    private readonly RoundStateStoreManager _roundStateStoreManager;

    public ApplicationCore(Scraper roundStatusScraper, IRoundDataReaderService dataProcessor,
        IRpcServerController rpcServerController, RoundStateStoreManager roundStateStoreManager)
    {
        _roundStatusScraper = roundStatusScraper;
        _dataProcessor = dataProcessor;
        _rpcServerController = rpcServerController;
        _roundStateStoreManager = roundStateStoreManager;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        Logger.LogInfo("Starting scraper...");
        await _roundStatusScraper.StartAsync(cancellationToken);
        await _roundStatusScraper.TriggerAndWaitRoundAsync(cancellationToken);
        Logger.LogInfo("Start reading rounds...");
        await _roundStatusScraper.ToBeProcessedData.Reader.WaitToReadAsync(cancellationToken);
        Logger.LogInfo("Start round data reader service...");
        await _dataProcessor.StartAsync(cancellationToken);
        Logger.LogInfo("Start round data saver...");
        await _roundStateStoreManager.StartAsync(cancellationToken);
        Logger.LogInfo("Start rpc server controller...");
        await _rpcServerController.StartRpcServerAsync(cancellationToken);
        Logger.LogInfo("Initialized, ready to work.");
        await Task.Delay(-1, cancellationToken);
    }
}