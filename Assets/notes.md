# Notes
The purpose of this file is to keep important development notes, e.g. things that worked or didn't work, bugs, workarounds etc.

Please future us, if you run into a problem, document it here!

### Build Issue for Android

We got this error when building to Android with Unity 2019.4.14.
Upgrading to 2020.1.15 solved it.
https://answers.unity.com/questions/1631097/android-build-error-value-cannot-be-null.html

### Build Issue for Android - Texture

Got some error saying "unsupported texture". It was something to do with Magic-Leap so we've removed the package and it worked.