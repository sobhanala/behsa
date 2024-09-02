# Stage 1: Base setup
FROM node:20-slim AS base

FROM base as test

WORKDIR /app

COPY /project/package*.json ./

RUN npm ci && npm install -g @angular/cli@18.1.4

# Install Google Chrome for headless testing
RUN apt-get update && apt-get install -y \
    wget \
    gnupg2 \
    && wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*  

ENV CHROME_BIN="/usr/bin/google-chrome"

# Stage 2: Test

COPY /project/ .

RUN ng test --watch=false --browsers=ChromeHeadlessNoSandbox

# Stage 3: Build
FROM base as build

WORKDIR /app

COPY /project/ .

RUN npm ci

# Build both the client and server-side parts
RUN npm run build


#FROM nginx:latest as nginx

#COPY nginx.conf /etc/nginx/nginx.conf

# Copy static assets from the build
#COPY --from=build /app/dist/project/browser/ /usr/share/nginx/html/



# Stage 4: Server
FROM base AS server

WORKDIR /app

# Copy the compiled server-side application from the build stage
COPY --from=build /app/dist/project/ /app/dist/

# Install only production dependencies
COPY /project/package*.json ./
RUN npm ci --only=production

EXPOSE 4000

# Start the Node.js server to handle SSR
CMD ["node", "dist/server/server.mjs"]


