# Screenshot

![POCSAG decoder](https://i.imgur.com/PsVOdEH.png)

# Installation

Extract the 'SdrSharpPocsagPlugin' folder in the release zip into the SDR# 'Plugins' directory.

![Plugin directory](https://i.imgur.com/5i2CYyo.png)

# Build from source

Use Visual Studio 2019, the debug output folder can be put into the SDR# 'Plugins' directory.

# Usage

Provided the plugin has been loaded successfully, there should be an option to display it in the main menu.

NFM with 12kHz bandwidth seems to work reliably, input is taken directly from the demodulator so audio filtering / mute shouldn't affect decoding.

![Plugin directory](https://i.imgur.com/9eGnJ9k.png)

## Options

### De-duplicate
The payload of each message is hashed (ignoring the first 9 characters, which seem to often include time data). With this option enabled new messages received with the same hash will be ignored.

### Hide bad decodes
Any messages that have failed BCH (CRC) or parity checks will be ignored.

### Clear
Will remove all messages from the buffer.

# Notes

* A maximum of 1000 messages will be displayed in the buffer, on a first-in-first-out basis.
* If a message payload is empty it is assumed to be tone message.
* If a message payload has fewer than 16 characters numerically, it is assumed to be a numeric message. In this case the numeric decode will be displayed first with the alphanumeric decode following e.g. "123456 (ALPHA: ABC)"
