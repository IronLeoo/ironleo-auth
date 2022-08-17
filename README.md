# ironleo-auth
#### A mobile app to generate authentication tokens for IronLeo applications

Requirement for guest-login and register of [IronLeo API](https://github.com/IronLeoo/ironleo-api)

## Use
When installed on the phone, navigate to the `settings` by pressing the icon in the top right corner.

Then enter the `server key`, `client key`, `crypto iv` and `api enpoint url` into the text boxes and hit `submit`.

These need to match your **IronLeo API config file**.

![image](https://user-images.githubusercontent.com/59258740/185197230-c49e7b33-c92e-490e-a84f-265d04788384.png)

To add new tokens, hit the `plus-sign` at the top of the home page.

There you can select the **type** and **expiration date** of the new token.

When you add a new token, you'll see a new entry on the main page that looks like this:

![image](https://user-images.githubusercontent.com/59258740/185198013-61126e26-f919-4354-b9bf-c2dd8db9f347.png)

It displays the following information:
1. Type eg. Cloudguest
2. Token
3. Expiration date
4. Time until expiration

At the right is a button to delete the token.

To refresh, simply pull down the page like any other app.
