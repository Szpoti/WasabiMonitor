﻿namespace WabiSabiMonitor.ApplicationCore.Interfaces;

public interface IRpcServerController
{
    Task StartRpcServerAsync(CancellationToken cancel);
}