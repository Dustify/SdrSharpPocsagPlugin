# Screenshot

![POCSAG decoder](https://i.imgur.com/WJaRTUd.png)

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

### Wrap payload
The payload section of the table will wrap rather than expand horizontally, newlines in payload data will also be respected.

### POCSAG 512 / 1200 / 2400 filter depths
Three controls are available to adjust the length of the moving average filter for each decoder. The filter aims to help decode noisy signals.

These values should be set as low as possible otherwise the data may be 'smoothed into incoherence'.

The filters can essentially be disabled by using a value of '1'.

### Clear
Will remove all messages from the buffer.

# Notes

* A maximum of 1000 messages will be displayed in the buffer, on a first-in-first-out basis.
* If a message payload is empty it is assumed to be tone message.
* If a message payload has fewer than 16 characters numerically, it is assumed to be a numeric message. In this case the numeric decode will be displayed first with the alphanumeric decode following e.g. "123456 (ALPHA: ABC)"
