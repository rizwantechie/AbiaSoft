#!/usr/bin/env python3
import argparse
import re
from pathlib import Path


STATIC_QUOTE_NOTICE = """<div class="static-quote-contact bg-white rounded p-4 text-center">
    <h4 class="text-primary mb-3">Request A Quote</h4>
    <p class="mb-4">This hosted website cannot submit forms. Email us about your project and our team will get back to you.</p>
    <a class="btn btn-dark py-3 px-4" href="mailto:abiasofttechnology@gmail.com?subject=Quote%20request">Email us for a quote</a>
</div>"""


def prepare_page(source: Path, destination: Path, base_path: str, is_homepage: bool) -> None:
    html = source.read_text(encoding="utf-8")
    if is_homepage:
        html, replaced = re.subn(
            r'<form\b(?=[^>]*\bid=["\']quoteForm["\'])[^>]*>.*?</form>',
            STATIC_QUOTE_NOTICE,
            html,
            count=1,
            flags=re.IGNORECASE | re.DOTALL,
        )
        if replaced != 1:
            raise RuntimeError("Could not replace the server-side quote form for static hosting.")

    base_path = "/" + base_path.strip("/") + "/"

    def rewrite_root_url(match: re.Match[str]) -> str:
        path = match.group(2)
        if path.lower() in {"privacy", "privacy/"}:
            path = "Privacy/"
        return f"{match.group(1)}{base_path}{path}"

    html = re.sub(
        r"""((?:href|src)=["'])/(?!/)([^"']*)""",
        rewrite_root_url,
        html,
        flags=re.IGNORECASE,
    )

    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(html, encoding="utf-8")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--homepage", type=Path, required=True)
    parser.add_argument("--privacy", type=Path, required=True)
    parser.add_argument("--site-dir", type=Path, required=True)
    parser.add_argument("--base-path", required=True)
    args = parser.parse_args()

    args.site_dir.mkdir(parents=True, exist_ok=True)
    prepare_page(args.homepage, args.site_dir / "index.html", args.base_path, True)
    prepare_page(
        args.privacy,
        args.site_dir / "Privacy" / "index.html",
        args.base_path,
        False,
    )


if __name__ == "__main__":
    main()
