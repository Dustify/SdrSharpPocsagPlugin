# Screenshot

![POCSAG decoder](https://i.imgur.com/2iYM8Z7.png)

# Installation

Extract the 'SdrSharpPocsagPlugin' folder in the release zip into the SDR# 'Plugins' directory.

![Plugin directory](https://i.imgur.com/5i2CYyo.png)

# Build from source

Use Visual Studio 2019, the debug output folder can be put into the SDR# 'Plugins' directory.

# Usage

Provided the plugin has been loaded successfully, there should be an option to display it in the main menu.

NFM with 12kHz bandwidth seems to work reliably, input is taken directly from the demodulator so audio filtering / mute shouldn't affect decoding.

![Plugin directory](https://i.imgur.com/9eGnJ9k.png)