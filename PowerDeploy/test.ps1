$elapsedTime = [system.diagnostics.stopwatch]::StartNew()

1..1000 | %{write-progress -activity "Working..." -status "$([string]::Format("Time Elapsed: {0:d2}:{1:d2}:{2:d2}", $elapsedTime.Elapsed.hours, $elapsedTime.Elapsed.minutes, $elapsedTime.Elapsed.seconds))" -percentcomplete ($_/10);}

$elapsedTime.stop()