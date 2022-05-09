#!/bin/sh


rm -rf tests/PerformantReflection.UnitTests/TestResults
dotnet build
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
