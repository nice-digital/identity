FROM node:8.10.0

RUN mkdir -p /tests

WORKDIR /tests

COPY package.json /tests
COPY package-lock.json /tests

RUN npm i

COPY . /tests
