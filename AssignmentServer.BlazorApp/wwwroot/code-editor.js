function InitializeEditor(theme, language) {
    const container = document.querySelector(".editor-container");

    if (!container) {
        console.error("cannot find a proper editor container");
        return;
    }

    if (!theme)    theme = "vs";
    if (!language) language = "c";

    window.editor = monaco.editor.create(container, {
        theme: theme,
        language: language
    });

    container.addEventListener("keydown", function (e) {
        if (e.ctrlKey) {
            if (e.code === "KeyS")
                document.querySelector("#ctSave").click();
            else if (e.code === "KeyE")
                document.querySelector("#ctExecute").click();
            else if (e.code === "KeyU")
                document.querySelector("#ctSubmit").click();

            e.preventDefault();
        }
    });

    UpdateEditorLanguage(language);
    UpdateEditorTheme(theme);
}

function UpdateEditorLanguage(lang) {
    if (!window.editor) return;

    const model = monaco.editor.createModel(editor.getValue(), lang);
    editor.setModel(model);
}

function UpdateEditorTheme(theme) {
    if (!window.editor) return;
    editor.updateOptions({ theme: theme });
}

function FinalizeUpdate() {

}