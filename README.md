# Ektron 51Degrees Template Switcher

***Note that this is written and tested against a fully updated Ektron 9.1 instance. Your mileage may vary.***

An alternative to using WURFL to control template switching / URL rewrites with Ektron CMS. Attempts to more-or-less replicate what Ektron provided, but without the neato admin interface. I may add such an interface later, if there's sufficient demand.

I'm basing this code on the assumption that developers are formulaic in the way they name desktop vs. mobile templates. In the sample, I assume *default.aspx* has a mobile-equivalent of *defaultmobile.aspx*, for example.

## Steps

Open your Ektron project in Visual Studio and open the Nuget Package manager. Install **51Degrees Mobile Detection and Optimization**

This installs some local data files, DLLs, a new /Mobile/Default.aspx template (which we won't use) and a 51Degrees.config file. It also makes some changes to your web.config.

***Before proceeding, follow the [instructions in our customer portal](https://portal.ektron.com/KB/10742/) to remove the WURFL integration from your site.***

### App_Code

Open `App_Code/CSCode/HttpModules`, included by default in your Ektron installation. Create a new folder here titled `eGandalf`. Copy the MobileDeviceModule class from GitHub into this folder. (Or give it a new namespace and put it in the folder of your choice, just remember that changing the namespace affects registering the module later in these instructions.)

### web.config changes
First, open your web.config and scroll down to the `<modules>` section. Look for this newly added code (might be on one line).

```xml
<remove name="Detector" />
<add name="Detector" type="FiftyOne.Foundation.Mobile.Detection.DetectorModule, FiftyOne.Foundation" />
```

And move them to the top of your `<modules>` section. This helps make sure that the BeginRequest code from 51Degrees will fire *before* the BeginRequest code in the `eGandalf.MobileDeviceModule` class.

Comment the following Ektron default module only *after* you've disabled the WURFL integration by following the instructions in our customer portal.

```xml
<!--<add name="EkMobileDeviceModule" type="Ektron.Cms.Settings.UrlAliasing.MobileDeviceModule" preCondition="integratedMode" />-->
```

In its place, add the following:

```xml
<add name="EgMobileDeviceModule" type="eGandalf.MobileDeviceModule" preCondition="integratedMode" />
```

### 51Degrees.config changes

Next, open the new 51Degrees.config file. Unless you plan to use 51Degrees' own redirection logic (which you are welcome do to), look for each `<location>` node in the `<redirect>` section and either remove them or set `enabled="false"` on each. Also remove the default `mobileHomePageUrl="~/Mobile/Default.aspx"` attribute (the whole thing) in the `<redirect>` node. This disables the default redirection logic so we can use our own in the class above.

Finally, scroll to the bottom of the file and set `enabled="false"` in the `<imageOptimisation>` node to prevent 51Degrees from inserting their own Javascript into the page (leave this enabled if you want to use their device detection client-side).

You should now be rewriting URLs. Make sure to modify the string modification logic to fit your own patterns.
