(() => {
  const body = document.body;
  const nav = document.querySelector('.nav-compass');
  const navLinks = [...document.querySelectorAll('[data-nav-link]')];
  const navDescription = document.getElementById('nav-description');
  const navToggle = document.querySelector('[data-nav-toggle]');
  const commandPalette = document.getElementById('command-palette');
  const commandOpeners = [...document.querySelectorAll('[data-palette-open]')];
  const commandCloser = commandPalette?.querySelector('[data-palette-close]');
  const commandInput = document.getElementById('command-palette-input');
  const commandResults = document.getElementById('command-palette-results');
  const commandStatus = document.getElementById('command-palette-status');
  const themeToggle = document.querySelector('[data-theme-toggle]');
  const readingRail = document.querySelector('[data-rail]');
  const docCanvas = document.querySelector('[data-doc-canvas]');

  const navData = window.__RC_NAV__ || [];
  const shortcuts = new Map();

  const setNavDescription = (text) => {
    if (navDescription) {
      navDescription.textContent = text;
    }
  };

  navLinks.forEach((link) => {
    const description = link.dataset.description;
    link.addEventListener('mouseenter', () => setNavDescription(description || ''));
    link.addEventListener('focus', () => setNavDescription(description || ''));
    link.addEventListener('mouseleave', () => setNavDescription('Hover a page in the Compass to read its story.'));
    link.addEventListener('blur', () => setNavDescription('Hover a page in the Compass to read its story.'));
  });

  const mobileNav = {
    open: () => {
      if (nav) {
        nav.dataset.open = 'true';
      }
    },
    close: () => {
      if (nav) {
        nav.dataset.open = 'false';
      }
    },
    toggle: () => {
      if (nav) {
        const open = nav.dataset.open === 'true';
        nav.dataset.open = open ? 'false' : 'true';
      }
    }
  };

  if (navToggle) {
    navToggle.addEventListener('click', () => mobileNav.toggle());
  }

  const palette = {
    visible: false,
    open() {
      if (!commandPalette) return;
      commandPalette.setAttribute('aria-hidden', 'false');
      document.body.style.overflow = 'hidden';
      commandInput?.focus();
      this.visible = true;
      renderCommandPalette('');
    },
    close() {
      if (!commandPalette) return;
      commandPalette.setAttribute('aria-hidden', 'true');
      document.body.style.overflow = '';
      commandInput?.blur();
      this.visible = false;
    },
    toggle() {
      if (this.visible) {
        this.close();
      } else {
        this.open();
      }
    }
  };

  commandOpeners.forEach((button) => button.addEventListener('click', () => palette.open()));
  commandCloser?.addEventListener('click', () => palette.close());

  document.addEventListener('keydown', (event) => {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      event.preventDefault();
      palette.toggle();
    }

    if (event.key === 'Escape' && palette.visible) {
      palette.close();
    }
  });

  let activeCommandIndex = 0;

  const getVisibleCommands = () => {
    return [...commandResults.querySelectorAll('.command-group__link')];
  };

  const moveCommandFocus = (direction) => {
    const commands = getVisibleCommands();
    if (!commands.length) return;

    activeCommandIndex = (activeCommandIndex + direction + commands.length) % commands.length;
    commands.forEach((cmd, index) => cmd.classList.toggle('is-active', index === activeCommandIndex));
    commands[activeCommandIndex].scrollIntoView({ block: 'nearest' });
  };

  const activateCommand = () => {
    const commands = getVisibleCommands();
    commands[activeCommandIndex]?.click();
  };

  const renderCommandPalette = (query) => {
    if (!commandResults) return;

    const normalized = query.trim().toLowerCase();

    const groups = navData.map((group) => {
      const filteredLinks = group.links.filter((link) => {
        if (!normalized) return true;
        return (
          link.label.toLowerCase().includes(normalized) ||
          (link.description && link.description.toLowerCase().includes(normalized))
        );
      });

      return { ...group, links: filteredLinks };
    }).filter((group) => group.links.length > 0);

    commandResults.innerHTML = '';

    if (groups.length === 0) {
      commandResults.innerHTML = '<p class="command-empty">No matches yet. Try a narrower phrase.</p>';
      commandStatus.textContent = 'No matches';
      return;
    }

    const fragment = document.createDocumentFragment();

    groups.forEach((group, groupIndex) => {
      const section = document.createElement('section');
      section.className = 'command-group';
      section.setAttribute('role', 'group');
      section.setAttribute('aria-labelledby', `command-group-${groupIndex}`);

      const header = document.createElement('header');
      header.innerHTML = `
        <span class="command-group__icon">${group.icon || ''}</span>
        <span id="command-group-${groupIndex}">${group.title}</span>
      `;
      section.appendChild(header);

      const list = document.createElement('ul');
      list.className = 'command-group__list';

      group.links.forEach((link, linkIndex) => {
        const item = document.createElement('li');
        item.className = 'command-group__item';

        const anchor = document.createElement('a');
        anchor.className = 'command-group__link';
        anchor.href = link.path;
        anchor.setAttribute('role', 'option');
        anchor.dataset.commandIndex = `${groupIndex}-${linkIndex}`;
        anchor.innerHTML = `
          <span class="command-group__meta">
            <span class="command-group__title">${link.label}</span>
            <span class="command-group__desc">${link.description || ''}</span>
          </span>
          <span class="command-group__key">↵</span>
        `;

        anchor.addEventListener('click', (event) => {
          event.preventDefault();
          window.location.href = anchor.href;
          palette.close();
        });

        item.appendChild(anchor);
        list.appendChild(item);
      });

      section.appendChild(list);
      fragment.appendChild(section);
    });

    commandResults.appendChild(fragment);
    const total = groups.reduce((sum, group) => sum + group.links.length, 0);
    commandStatus.textContent = `${total} result${total === 1 ? '' : 's'}`;
    activeCommandIndex = 0;
    getVisibleCommands()[0]?.classList.add('is-active');
  };

  commandInput?.addEventListener('input', (event) => {
    renderCommandPalette(event.target.value);
  });

  commandInput?.addEventListener('keydown', (event) => {
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      moveCommandFocus(1);
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      moveCommandFocus(-1);
    } else if (event.key === 'Enter') {
      event.preventDefault();
      activateCommand();
    }
  });

  const initializeShortcuts = () => {
    shortcuts.set('slash', (event) => {
      if (event.target === commandInput) {
        event.stopPropagation();
      }
    });

    document.addEventListener('keydown', (event) => {
      if (event.key === 't' && !['INPUT', 'TEXTAREA'].includes(event.target.tagName)) {
        event.preventDefault();
        toggleTheme();
      }
    });
  };

  const toggleTheme = () => {
    const current = localStorage.getItem('rc-theme') || 'auto';
    const next = current === 'light' ? 'dark' : current === 'dark' ? 'auto' : 'light';
    applyTheme(next);
  };

  const applyTheme = (mode) => {
    localStorage.setItem('rc-theme', mode);
    body.classList.remove('theme-light', 'theme-dark');

    if (mode === 'auto') {
      body.classList.add(window.matchMedia('(prefers-color-scheme: dark)').matches ? 'theme-dark' : 'theme-light');
    } else {
      body.classList.add(mode === 'dark' ? 'theme-dark' : 'theme-light');
    }
  };

  themeToggle?.addEventListener('click', () => toggleTheme());
  applyTheme(localStorage.getItem('rc-theme') || 'auto');

  const buildToc = () => {
    if (!readingRail || !docCanvas) return;

    const headings = docCanvas.querySelectorAll('h2, h3');
    if (!headings.length) return;

    const toc = document.createElement('nav');
    const list = document.createElement('ul');
    toc.appendChild(list);

    headings.forEach((heading) => {
      if (!heading.id) {
        heading.id = heading.textContent.toLowerCase().replace(/[^a-z0-9]+/g, '-');
      }

      const item = document.createElement('li');
      const link = document.createElement('a');
      link.href = `#${heading.id}`;
      link.textContent = heading.textContent;
      link.dataset.tocLink = heading.id;

      link.addEventListener('click', (event) => {
        event.preventDefault();
        document.getElementById(heading.id)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
        palette.close();
      });

      item.appendChild(link);
      list.appendChild(item);
    });

    const railToc = document.getElementById('rail-toc');
    if (railToc) {
      railToc.innerHTML = '';
      railToc.appendChild(toc);
    }

    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        const link = document.querySelector(`[data-toc-link="${entry.target.id}"]`);
        if (!link) return;
        if (entry.isIntersecting) {
          document.querySelectorAll('[data-toc-link]').forEach((node) => node.classList.remove('is-active'));
          link.classList.add('is-active');
        }
      });
    }, {
      rootMargin: '-40% 0px -45% 0px',
      threshold: 0.1
    });

    headings.forEach((heading) => observer.observe(heading));
  };

  const hydrateActiveNav = () => {
    const page = document.body.dataset.page;
    if (!page) return;
    navLinks.forEach((link) => {
      const href = link.getAttribute('href');
      if (!href) return;
      const normalizedHref = href.replace(window.location.origin, '').replace(/index\.html$/, '/');
      const normalizedPage = page.replace(/index\.html$/, '/');
      if (normalizedHref === normalizedPage) {
        link.classList.add('is-active');
        link.closest('[data-stack]')?.setAttribute('data-active', 'true');
      }
    });
  };

  const installNavData = () => {
    if (!window.__RC_NAV__) {
      window.__RC_NAV__ = navData;
    }
  };

  installNavData();
  initializeShortcuts();
  renderCommandPalette('');
  buildToc();
  hydrateActiveNav();

  window.addEventListener('resize', () => {
    if (window.innerWidth > 980) {
      nav?.removeAttribute('data-open');
    }
  });
})();

