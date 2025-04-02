FROM node:18-slim AS base
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"

# https://github.com/misskey-dev/misskey/issues/15386#issuecomment-2629716327
ENV COREPACK_DEFAULT_TO_LATEST=0

RUN corepack enable
COPY ./ui /app
WORKDIR /app

FROM base AS prod-deps
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --prod --frozen-lockfile

FROM base AS build
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --frozen-lockfile
RUN pnpm run build-docker

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html/
COPY ./ui/nginx/variables.conf.template /etc/nginx/templates/variables.conf.template
COPY ./ui/nginx/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80