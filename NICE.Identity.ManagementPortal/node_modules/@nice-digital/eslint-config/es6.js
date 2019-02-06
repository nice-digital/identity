module.exports = {
  "env": {
    "browser": true,
    "node": true,
    "es6": true,
    "commonjs": true
  },
  "globals": {
  	"process": false,
  	"$": false,
  	"PRODUCTION": false
  },
  "extends": "eslint:recommended",
  "parserOptions": {
    "sourceType": "module"
  },
  "rules": {
    "no-console": 0,
    "no-unused-vars": "warn",
    "indent": [
      "error",
      "tab",
      { "SwitchCase": 1 }
    ],
    "linebreak-style": 0,
    "quotes": [
      "error",
      "double"
    ],
    "semi": [
      "error",
      "always"
    ]
  }
};