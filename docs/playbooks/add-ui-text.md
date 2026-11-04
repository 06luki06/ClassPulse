# Add UI text

1. Look for an existing key in `ClassPulse/Resources/Resources.resx`, and reuse it if the meaning matches.
2. Otherwise, add the key to both files:
   - `Resources.resx`: English
   - `Resources.de.resx`: German
3. Name the key `Label_<Name>`, `Dialog_<DialogName>_<Name>`, `General_<Name>` or, for text used by one view, with that view's prefix, such as `Home_<Name>`.
4. Add the matching property with a `<summary>` to `Resources.Designer.cs`.
   - Use the `PublicResXFileCodeGenerator` in an IDE if available.
   - The file is generated, so its pattern must stay identical if it is edited without an IDE. Tell the user if you did this.
5. Use the text:
   - AXAML: `{x:Static res:Resources.Label_Name}`, with `xmlns:res="clr-namespace:At.luki0606.ClassPulse.Resources"`
   - C#: `Resources.Resources.Label_Name`
6. Run the tests. `Tests/ResourcesTests.cs` fails if a key is missing from a language or has a blank value.

`InputDialogViewModel.GetValue` finds fields by their label, so labels within a dialog must be unique in both languages.
