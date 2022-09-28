const withTM = require("next-transpile-modules")(["ui"]);

module.exports = withTM({
  reactStrictMode: true,
  images: {
    formats: ['image/avif', 'image/webp']
  }
});
