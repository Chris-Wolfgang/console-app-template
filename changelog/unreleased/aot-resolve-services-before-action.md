type: fix

The native-AOT template resolves its services before defining the command action, so the action no longer captures the disposable host.
