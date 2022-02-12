using Microsoft.AspNetCore.Components.Forms;

namespace SuggestionAppUI.Components;

// TC did this as he said there was some issue requiring him to double click the radio button to register the selection.
// After he typed some data into an input field and then tried to select a radio button it was not registering the 
// selection without him having to click twice.
public class MyInputRadioGroup<TValue> : InputRadioGroup<TValue>
{
    private string _name;
    private string _fieldClass;

    protected override void OnParametersSet()
    {
        var fieldClass = EditContext?.FieldCssClass(FieldIdentifier) ?? string.Empty;
        if (fieldClass != _fieldClass || Name != _name)
        {
            _fieldClass = fieldClass;
            _name = Name;
            base.OnParametersSet();
        }
    }
}
