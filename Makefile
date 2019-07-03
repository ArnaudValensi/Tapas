all: build

build:
	@msbuild /p:Configuration=Debug /nologo /verbosity:quiet Tapas.Tests/Tapas.Tests.csproj

run: 
	@mono --debug Tapas.Tests/bin/Debug/Tapas.exe

clean:
	@find . -type d -name "bin" -o -name "obj" | xargs rm -fr

.PHONY: all build run clean
