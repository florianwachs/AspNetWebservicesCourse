FROM postgres:18-bookworm

ARG PG_MAJOR=18
ARG PGVECTOR_VERSION=v0.8.2
ARG PGTEXTSEARCH_VERSION=v1.0.0
ARG PGVECTORSCALE_VERSION=0.9.0
ARG APACHE_AGE_VERSION=PG18/v1.7.0-rc0

ENV DEBIAN_FRONTEND=noninteractive
ENV RUSTUP_HOME=/usr/local/rustup
ENV CARGO_HOME=/usr/local/cargo
ENV PATH=/usr/local/cargo/bin:${PATH}

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    bison \
      build-essential \
      clang \
      curl \
      ca-certificates \
    flex \
      git \
      jq \
    libreadline-dev \
      pkg-config \
      libssl-dev \
    postgresql-server-dev-${PG_MAJOR} \
    zlib1g-dev && \
    rm -rf /var/lib/apt/lists/*

RUN curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | \
    sh -s -- -y --profile minimal

RUN git clone --branch ${PGVECTOR_VERSION} --depth 1 https://github.com/pgvector/pgvector.git /tmp/pgvector && \
    cd /tmp/pgvector && \
    make && \
    make install && \
    rm -rf /tmp/pgvector

RUN git clone --branch ${PGTEXTSEARCH_VERSION} --depth 1 https://github.com/timescale/pg_textsearch.git /tmp/pg_textsearch && \
    cd /tmp/pg_textsearch && \
    make && \
    make install && \
    rm -rf /tmp/pg_textsearch

RUN git clone --branch ${PGVECTORSCALE_VERSION} --depth 1 https://github.com/timescale/pgvectorscale.git /tmp/pgvectorscale && \
    cd /tmp/pgvectorscale/pgvectorscale && \
    cargo install --locked cargo-pgrx --version "$(cargo metadata --format-version 1 | jq -r '.packages[] | select(.name == "pgrx") | .version')" && \
    cargo pgrx init --pg18 "$(command -v pg_config)" && \
    cargo pgrx install --release && \
    rm -rf /tmp/pgvectorscale

RUN git clone --branch ${APACHE_AGE_VERSION} --depth 1 https://github.com/apache/age.git /tmp/age && \
    cd /tmp/age && \
    make PG_CONFIG="$(command -v pg_config)" && \
    make PG_CONFIG="$(command -v pg_config)" install && \
    rm -rf /tmp/age

COPY ./postgresql-extra.conf /usr/local/share/postgresql/postgresql-extra.conf
RUN printf "\ninclude_if_exists = '/usr/local/share/postgresql/postgresql-extra.conf'\n" >> "$(pg_config --sharedir)/postgresql.conf.sample"
COPY ./init/00-extensions.sql /docker-entrypoint-initdb.d/00-extensions.sql
