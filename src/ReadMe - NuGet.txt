If you're going to be compiling this solution from the source, be aware that the NuGet package for
Newtonsoft.Json shouldn't be updated from its current version of 8.0.3

This isn't because of API compatability, but rather efficiency. Since this is a plugin, it runs in the
context of Visual Studio which already loads Newtonsoft.Json. While the designer continues to support
Visual Studio 2017, it should load Newtonsoft.Json v8.0.3, the version that VS2017 already has loaded.
That way there aren't two versions of the same package loaded into memory.

When we drop VS2017, we'll bump that version up to 9.0.1, the minimum version that's loaded by any
point release of VS2019.

For more information, please see https://devblogs.microsoft.com/visualstudio/using-newtonsoft-json-in-a-visual-studio-extension/

