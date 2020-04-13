### What is this project?


### Alternatives

- [StormLib](https://github.com/ladislav-zezula/StormLib) is the most widely used reader and writer for MPQ files.
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) is a .NET wrapper for StormLib.

### MPQ File Format Specifications

The following sources were used to understand the MPQ format and develop this library.

- http://www.zezula.net/en/mpq/mpqformat.html
- https://github.com/ladislav-zezula/StormLib


### Unsupported Features

The following features are not currently supported, but I could support in the future
if needed.

- Using the value of Locale in HashTable.
- Compressed HET & BET tables.
- Searching from HET & BET tables.
- Part Archives and Patches.
- Archives that do not start with UserData.
- Encrypted files.
- Imploded files.
- Compressed files.
- Files (within the archive) larger than 4GB