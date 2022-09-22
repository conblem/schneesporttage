FROM cloudflare/cloudflared:latest as bin

FROM alpine:latest

WORKDIR /app

COPY --from=bin /usr/local/bin/cloudflared ./

ENTRYPOINT ["/app/cloudflared", "--no-autoupdate"]
CMD ["version"]