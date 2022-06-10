const colors = require("tailwindcss/colors");

let theme =
{
    colors: colors
};

theme.colors["dark"] = "#171721";
theme.colors["light"] = "#ffffff";

theme.colors["primary"] = "#63dd3a";
theme.colors["primary-alt"] = "#52cc29";
theme.colors["primary-inv"] = "#ffffff";
theme.colors["primary-light"] = "#63dd3a10";

theme.colors["secondary"] = colors.pink["500"];
theme.colors["secondary-inv"] = colors.pink["100"];

theme.colors.muted = colors.slate["400"];

module.exports =
{
    content:
    [
        "./index.html",
        "./src/**/*.{html,js,ts,jsx,tsx,vue}"
    ],
    theme: theme,
    plugins: []
}
