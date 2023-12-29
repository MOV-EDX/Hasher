# Hasher

Hasher is a very simple hashing tool which will calculate the SHA512 (including Base64), SHA256 (including Base64), SHA1 (including Base64) and MD5 hashes for a given file or set of files. You can simply drag and drop a file or directory over the tool and it will compute the hashes for you. All output is displayed in a console window.

**Using Hasher to verify hashes against a CBS log**

Hasher will now verify hashes in a source directory against those mentioned in a CBS log using the following syntax:

Hasher -source <source_path> -log <log_path>

Any files in the source directory which do not match the expected hash will be printed to the console.

**Adding Hasher to the Context Menu**

If you wish to have ability to right-click a file or directory, then please import the following as a .reg file:

<code>Windows Registry Editor Version 5.00
[HKEY_CLASSES_ROOT\Directory\shell\Hasher\command]
@="\"C:\\Tools\\Hasher\\Hasher.exe\" \"%1\""</code>
