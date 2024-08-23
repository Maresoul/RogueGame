----------------------------------------
POWERUP FX
----------------------------------------

1. Introduction
2. Customizing colors 
3. Customizing emission & glowyness
4. Scaling effects
5. URP Upgrade
6. Contact

----------------------------------------
1. INTRODUCTION
----------------------------------------

To use the effects, simply find a way to instantiate them, or drag & drop them into the scene.

The effects will automatically start playing when the scene is running.

----------------------------------------
2. CUSTOMIZING COLORS
----------------------------------------

Which colors the effects are using, are all set in the Start Color at the top of the particle system.

Some effects also use gradient in the Color over Lifetime module to gradually modulate the colors over their lifetime.

----------------------------------------
3. CUSTOMIZING EMISSION & GLOWYNESS
----------------------------------------

In order to view the glowing effects of emission, make sure that you find a post processing solution that supports Bloom.

For some versions of Unity, you can find a Post-Processing Stack on the Unity Asset Store, or in later versions, a Post-Processing solution should be available in the Package Manager.

If you wish to change the glow for certain parts of effects, locate the material you wish to edit in the Materials folder. Take for example the 'Materials/Symbol/SymbolNeon' material.

You can see here that the Albedo is increased from the default 1.0 to 4.91, this will significantly boost the glowyness of the white parts in the albedo texture. Since this is a particle shader, it will inherit the color used in the Particle System settings.

In some materials, Emission is also enabled with a white or grey color. This will add an additional boost of that specified color. Enabling both Albedo color with a monochrome color in Emission will blend the colors and make it look washed out.

----------------------------------------
4. SCALING EFFECTS
----------------------------------------

To scale an effect in the scene, simply use the default Scaling tool (Hotkey 'R'). You can also select the effect and type in the Scale in Transform manually.

----------------------------------------
5. URP Upgrade
----------------------------------------

To upgrade to URP, simply find the 'Powerup FX/Upgrades' folder and apply the 'URP Upgrade' unitypackage to your project.

If you have any problems, please make sure you've installed the latest version of Universal Render Pipeline and Post Processing in the package manager.

----------------------------------------
6. CONTACT
----------------------------------------

Twitter: @archanor
Support: archanor.work@gmail.com

For support or criticism, please contact me over e-mail.

Follow me and my work on Twitter for updates and new VFX assets.

Ratings & reviews are much appreciated!

----------------------------------------

Made by Kenneth "Archanor" Foldal Moe
