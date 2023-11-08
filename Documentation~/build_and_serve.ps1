git clean -xdf
docfx metadata .\docfx.json
docfx build .\docfx.json
docfx serve .\_site\
