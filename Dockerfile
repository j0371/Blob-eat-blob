
#Layer 1---------------------------------------------------------------------
# Use a minimal, current Python 3.12 base image suitable for development
FROM python:3.12-slim-bookworm

# Update package lists and install essential build and runtime dependencies
# ca-certificates: for TLS trust when downloading packages
# curl, wget: for downloading tools (e.g., Copilot CLI)
# git: version control system
# gnupg: needed for verifying GitHub CLI package signatures
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    wget \
    git \
    gnupg \
    && rm -rf /var/lib/apt/lists/*

#Layer 2---------------------------------------------------------------------
# Install GitHub CLI (gh) from the official repository
# This layer adds the GitHub CLI key and repository, then installs gh
RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | gpg --dearmor -o /usr/share/keyrings/githubcli-archive-keyring.gpg && \
    printf 'deb [arch=%s signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main\n' "$(dpkg --print-architecture)" > /etc/apt/sources.list.d/github-cli.list && \
    apt-get update && apt-get install -y --no-install-recommends gh && \
    rm -rf /var/lib/apt/lists/*

# Install the current stable GitHub Copilot CLI (standalone) from an explicit release asset
# Use a pinned release from the official copilot-cli repo for reproducibility
RUN curl -fsSL https://github.com/github/copilot-cli/releases/download/v1.0.88/copilot-linux-x64.tar.gz -o /tmp/copilot.tar.gz && \
    tar -xzf /tmp/copilot.tar.gz -C /tmp && \
    install -m 0755 /tmp/copilot /usr/local/bin/copilot && \
    rm -f /tmp/copilot /tmp/copilot.tar.gz

#Layer 3---------------------------------------------------------------------
# Create a non-root user 'agent' with fixed UID/GID for consistency
# This allows the user to read/write bind-mounted repositories on the host
RUN groupadd -g 1000 agent && \
    useradd -u 1000 -g 1000 -m -s /bin/bash agent

#Layer 4---------------------------------------------------------------------
# Create and own the /workspace directory
RUN mkdir -p /workspace && \
    chown -R agent:agent /workspace

# Set /workspace as the working directory
WORKDIR /workspace

# Default to a shell so interactive container use behaves like a development terminal
CMD ["/bin/bash"]

# Switch to the non-root user for all subsequent operations
USER agent
