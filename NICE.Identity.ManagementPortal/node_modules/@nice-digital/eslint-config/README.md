# eslint-config

> Shared ESLint config for use in NICE Digital Services

## Usage

```
npm i @nice-digital/eslint-config --save-dev
```

Then in your project's *.eslintrc*:

```json
{
    "extends": "@nice-digital/eslint-config"
}
```

Or to use ES6:

```json
{
    "extends": "@nice-digital/eslint-config/es6"
}
```

Or to use [FlowType annotations](https://flow.org/en/):

```sh
npm i babel-eslint eslint-plugin-flowtype --save-dev
```

```json
{
    "extends": "@nice-digital/eslint-config/es6-flow"
}
```