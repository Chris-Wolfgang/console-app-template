type: fix

Generated test, integration-test and benchmark projects now build in Release on the .NET 8 SDK: they suppress CS9057 (the McMaster source generator, which flows in through the app reference, targets a newer compiler) the same way the app project already does.
