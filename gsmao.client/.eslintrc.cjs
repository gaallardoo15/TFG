// npm run lint -- --fix
module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:react/jsx-runtime',
    'plugin:react-hooks/recommended',
    "plugin:jsx-a11y/recommended",
    "plugin:no-unsanitized/DOM",
    "plugin:prettier/recommended",
  ],
  "globals": {
    "$": false,
    "_": false,
    "gettext": false,
    "interpolate": false,
  },
  // "parser": "babel-eslint",
  ignorePatterns: ['dist', '.eslintrc.cjs'],
  parserOptions: { ecmaVersion: 'latest', sourceType: 'module' },
  settings: { react: { version: '18.2' } },
  // plugins: ['react-refresh'],
  plugins: ["react-refresh", "react", "react-hooks", "jsx-a11y", "no-unsanitized", "simple-import-sort", "unused-imports", "no-relative-import-paths"],
  rules: {
    'prettier/prettier': ['error', {
      "endOfLine": "crlf"
    }],
    'react/jsx-no-target-blank': 'off',
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    "arrow-body-style": "off",
    "curly": ["error", "all"],
    "default-case": "off",
    "dot-notation": "error",
    "eqeqeq": "off",
    "jsx-a11y/anchor-is-valid": "warn",
    "jsx-a11y/click-events-have-key-events": "warn",
    "jsx-a11y/no-autofocus": "off",
    "jsx-a11y/no-static-element-interactions": "warn",
    "jsx-a11y/no-noninteractive-element-interactions": "warn",
    "no-unsanitized/method": "error",
    "no-unsanitized/property": "error",
    "react/display-name": "warn",
    "react-hooks/exhaustive-deps": "warn",
    "react-hooks/rules-of-hooks": "error",
    "react/jsx-key": "error",
    "react/prop-types": "off",
    "semi": ["error", "always"],
    "no-undef": "warn",
    "simple-import-sort/imports": [
      "error",
      {
        "groups": [
          ["^react", "^@?\\w"], // React and top level
          ["^components/", "^\\.", "^utils/"], // Common components, local components, utils
          ["^\\u0000", "^"] // Imperatives and the rest
        ]
      }
    ],
    "template-curly-spacing": "off", // https://github.com/babel/babel-eslint/issues/681#issuecomment-420663038
    "unused-imports/no-unused-imports": "error",
    "no-relative-import-paths/no-relative-import-paths": [
      "warn",
      {
        "allowSameFolder": true,
        "rootDir": "src",
        "prefix": "@"
      }
    ]
  },
}
