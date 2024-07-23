# NMS Resign
## 
### Forked from [NMSResign](https://www.nexusmods.com/nomanssky/mods/1565) by stk25 on NexuxMods.
## 
This tool can recreate the BankSignatures.bin file for you based on your own NMSARC.pak files, which stops the game from showing any "File Tamper Warning" message.

I have updated it with some functionality changes and to fix a bug.

#### Usage
Place the executable in the PCBANKS folder or on PATH. Use the program from the command line or reference it in a script.

`-decrypt`:	Decrypts a .bin file to a .dec file.

`-encrypt`:	Encrypts a .dec file to a .bin file.

`-createbin`: Creates a new encrypted bin file from the folder of pak files.

`-createdec`: Creates a new decrypted file bin.dec file from the folder of pak files.

`-help`: Displays a help message.

All commands now also accept a filename as an additional argument, although this is mostly for testing purposes.