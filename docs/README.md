# Starflare Design Docs

The design docs site for Starflare Casino: characters, story timeline, outcomes, mysteries, the city and the rules. This folder is the source of truth for the game's design.

- `index.html` is the page shell, `style.css` the styles, `app.js` the pages, router and Markdown renderer.
- `data/story.js` holds the list, map and timeline data.
- `docs/` holds the page text as Markdown (`people/`, `pages/`, `plan/`); `docs/index.js` lists every file for search.

The Markdown is fetched at run time, so opening `index.html` from disk won't work. Serve the folder instead:

```sh
cd docs && python3 -m http.server 8000
```

Then open http://localhost:8000.
