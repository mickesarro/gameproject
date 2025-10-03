# UI system

[UIManager](./Code/UIManagers/UIManager.cs) is a [singleton](./SingletonBase.cs) the different UILayers can be shown through.

It uses a stack to keep track of previously shown layer history.

## Workflow
Add the UIManager to some GameObjects in the editor. There apply the start layer
to what is first shown, e.g. the HUD. All additional, including the start layer,
should be added to the UILayers list. That is not strictly necessary, but it enables
the use of generic ShowLayer methods. 

All UI layers shown through this system need to implement the abstract class UILayer.
This specifies the Show and Hide methods that the system uses. These can, and in
most cases, should be overridden.

In code it can be used through the static Instance property. The list can't be
as of now instantiated through code, but it is not strictly necessary.

The different types of show methods take additional boolean for storing the current
one in history. Beware of creating loops this way.

Example usage:
```
private void OpenSettings() {
    UIManager.Instance.ShowLayer<SettingsMenu>();
}

[Property] private SettingsMenu settingsMenu { get; set; }
private void OpenSettings() {
    UIManager.Instance.ShowLayer(settingsMenu);
}
```
## Code reuse
SingletonBase can be moved to some other directory such as common etc. if needed
elsewhere.
