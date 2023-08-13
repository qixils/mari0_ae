# Mari0: AE Crowd Control

This is the [Crowd Control](https://crowdcontrol.live/) mod for [Mari0: AE](https://github.com/alesan99/mari0_ae)
(which itself is a mod of [Mari0](https://github.com/Stabyourself/mari0)).

## Contributing

To test effects, you'll want to download the [Crowd Control SDK](https://developer.crowdcontrol.live/sdk/). In the SDK,
click Load Pack Source, select Mari0.cs, click Connect, then open the game or press F9. You will now be able to use the
SDK to send effects! Also check out Effects -> Autopilot to send random effects every second (though note this feature
is pretty buggy).

To create a new effect, you'll want to use the function `cc_ack(effect: string) -> boolean` to check if an effect is
active. Note that this method will mark the requested effects as having succeeded and thus will consume the purchaser's
coins, so **please be careful** to only call this method when you're certain the effect can be applied.
To use an effect, it needs to be defined in the game pack (Mari0.cs) file, namely in the Effects variable. A typical
effect looks something like:

```cs
new("Display Name", "code_name") { Price = 50, Duration = 15, Category = "Player", Description = "Does a thing!" },
```

Note that the price and duration fields are just defaults that streamers can change.
Also note that the category is anoptional field for narrowing down the list of effects. It can be whatever you want,
even `new("Multiple", "Categories")`!

![Screenshot of the category dropdown in the Crowd Control extension](https://github.com/qixils/mari0_ae/assets/13265322/f653ef93-a349-4cff-809c-e9bab54e67f5)

## Contributors

This mod was created by:

- @qixils: Wrote the networking code and several effects
