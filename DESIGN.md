---
name: Fluent Desktop
colors:
  surface: '#fbf9f8'
  surface-dim: '#dbdad9'
  surface-bright: '#fbf9f8'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f5f3f3'
  surface-container: '#efeded'
  surface-container-high: '#e9e8e7'
  surface-container-highest: '#e4e2e2'
  on-surface: '#1b1c1c'
  on-surface-variant: '#424752'
  inverse-surface: '#303030'
  inverse-on-surface: '#f2f0f0'
  outline: '#727783'
  outline-variant: '#c2c6d4'
  surface-tint: '#005db5'
  primary: '#00488d'
  on-primary: '#ffffff'
  primary-container: '#005fb8'
  on-primary-container: '#cadcff'
  inverse-primary: '#a8c8ff'
  secondary: '#006687'
  on-secondary: '#ffffff'
  secondary-container: '#60cdff'
  on-secondary-container: '#005572'
  tertiary: '#004b79'
  on-tertiary: '#ffffff'
  tertiary-container: '#00649e'
  on-tertiary-container: '#c0deff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#d6e3ff'
  primary-fixed-dim: '#a8c8ff'
  on-primary-fixed: '#001b3d'
  on-primary-fixed-variant: '#00468b'
  secondary-fixed: '#c1e8ff'
  secondary-fixed-dim: '#74d1ff'
  on-secondary-fixed: '#001e2b'
  on-secondary-fixed-variant: '#004d67'
  tertiary-fixed: '#cee5ff'
  tertiary-fixed-dim: '#97cbff'
  on-tertiary-fixed: '#001d33'
  on-tertiary-fixed-variant: '#004a77'
  background: '#fbf9f8'
  on-background: '#1b1c1c'
  surface-variant: '#e4e2e2'
typography:
  display:
    fontFamily: Inter
    fontSize: 68px
    fontWeight: '600'
    lineHeight: 92px
    letterSpacing: -0.02em
  title-lg:
    fontFamily: Inter
    fontSize: 40px
    fontWeight: '600'
    lineHeight: 52px
    letterSpacing: -0.015em
  title-lg-mobile:
    fontFamily: Inter
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
    letterSpacing: -0.01em
  title:
    fontFamily: Inter
    fontSize: 28px
    fontWeight: '600'
    lineHeight: 36px
    letterSpacing: -0.01em
  subtitle:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 26px
    letterSpacing: 0em
  body-large:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 22px
    letterSpacing: 0em
  body:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
    letterSpacing: 0em
  body-strong:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
    letterSpacing: 0em
  caption:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 16px
    letterSpacing: 0.01em
  caption-strong:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.01em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  gutter: 1rem
  gutter-sm: 0.5rem
  gutter-lg: 1.5rem
  margin: 1.5rem
  margin-sm: 1rem
  margin-lg: 2rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 0.75rem
  space-lg: 1rem
  space-xl: 1.5rem
---

## Brand & Style

This design system delivers a polished, responsive desktop-first environment inspired by modern operating system ergonomics. It emphasizes calm efficiency, spatial coherence, and material depth. The visual tone is precise, dependable, and quietly modern—balancing functional utility with tactile digital craftsmanship.

The visual style synthesizes modern glassmorphism and layered surface hierarchies through translucent materials, subtle elevation, and softly rounded geometries:
- **Materials over flat planes:** Surfaces communicate optical hierarchy through translucency, tinted backdrop filters, and subtle ambient light absorption.
- **Micro-elevation:** Restrained dropshadows paired with 1px semi-transparent borders define clear layer separation without visual clutter.
- **Ergonomics:** Hit targets, subtle state transitions (rest, hover, active/pressed), and deliberate spatial paddings prioritize high productivity across mouse, pen, and touch inputs.

## Colors

The color system is organized around structural neutrals and an expressive accent palette:

- **Accent Primary (`#005FB8`):** Serves as the primary operational color for call-to-action buttons, active selection pills, focus indicators, toggles, and key states. In dark contexts, this shifts to `#60CDFF` for optimal perceptual contrast.
- **Accent Interactive States:** Hover states apply a 10% brightness increase; pressed states apply a 15% deeper shade (`#004E98`).
- **Surface Materials:**
  - *Mica Background:* Dynamic base canvas tint reflecting system wallpaper with low saturation blur (`#F3F3F3` in light mode, `#202020` in dark mode).
  - *Acrylic / Flyout Surface:* 85% opacity background with a 30px backdrop blur (`rgba(255, 255, 255, 0.85)` / `rgba(44, 44, 44, 0.85)`).
  - *Card Surface Rest:* Solid or semi-translucent crisp white (`#FFFFFF` light / `#2B2B2B` dark) over the Mica canvas.
- **Borders & Dividers:** Subtle translucent stroke (`rgba(0, 0, 0, 0.08)` on light; `rgba(255, 255, 255, 0.09)` on dark) creating crisp structural bounding without heavy dark lines.

## Typography

The typography scale uses clean geometric grotesque letterforms configured to match variable display dynamics. 

- **Optical Weights:** Default text uses regular weight (`400`) for effortless sustained reading, stepping into semi-bold (`600`) for headings, column headers, and active UI states.
- **Hierarchy Mapping:**
  - `display` and `title-lg` are reserved for marquee hero headers and high-impact landing canvases.
  - `title` and `subtitle` anchor window pane headers, settings section titles, and modal dialog titles.
  - `body` (14px/20px) functions as the standard operating baseline for form labels, standard text, table cells, and tree views.
  - `caption` handles metadata, secondary timestamps, keyboard shortcuts, and descriptive sub-labels.
- **Rendering & Legibility:** Always render with subpixel antialiasing (`-webkit-font-smoothing: antialiased`). Keep letter spacing tight on large display headers while keeping captions neutral to slightly tracked.

## Layout & Spacing

The layout is structured around an integrated app window canvas featuring navigation panes, content frames, and standard caption bars:

- **Window Anatomy:**
  - *Title Bar (Caption Bar):* Standard 40px height fixed at the top, housing optional breadcrumbs/search centered or left-aligned, and system window controls (minimize, maximize, close) anchored to the top-right corner.
  - *Navigation Pane:* Left-docked navigation sidebar (collapsible between 48px compact icon rail and 240px expanded navigation tree).
  - *Content Frame:* Fluid canvas accommodating responsive card grids or data views.
- **Grid & Alignment:**
  - Master content views follow a fluid multi-column arrangement adapting between 4 columns on narrow viewports (< 648px), 8 columns on intermediate viewports (648px–1023px), and 12 columns on desktop viewports (≥ 1024px).
  - Internal layout rhythm adheres strictly to 4px and 8px increments.

## Elevation & Depth

Elevation conveys depth and visual separation through translucent materials, micro-strokes, and directional lighting:

- **Materials:**
  - **Mica Canvas:** Used for root window backgrounds. Creates a stationary, tinted material showing subtle color bleeding through windows.
  - **Acrylic:** Used for temporary, transient overlays (flyouts, contextual context menus, datepickers, tooltips). Implements backdrop blur (`20px` to `30px`) with a luminous gradient border stroke.
- **Lighting and Layer Strokes:**
  - All raised elements (cards, dialogs, inputs) feature a top-lit border highlight: a 1px composite outline using `border-top: 1px solid rgba(255, 255, 255, 0.2)` and `border-bottom: 1px solid rgba(0, 0, 0, 0.1)`.
- **Shadow Scale:**
  - *Resting Cards / Controls:* `0px 1px 2px rgba(0, 0, 0, 0.04), 0px 2px 4px rgba(0, 0, 0, 0.04)`.
  - *Hovered Interactive Surfaces:* `0px 2px 4px rgba(0, 0, 0, 0.06), 0px 4px 8px rgba(0, 0, 0, 0.06)`.
  - *Flyouts & Popovers:* `0px 4px 8px rgba(0, 0, 0, 0.08), 0px 8px 16px rgba(0, 0, 0, 0.12)`.
  - *Modal Dialogs:* `0px 8px 16px rgba(0, 0, 0, 0.14), 0px 32px 64px rgba(0, 0, 0, 0.18)`.

## Shapes

The shape hierarchy establishes consistent soft corners across all structural and interactive items:

- **Control Geometry (4px):** Standard controls such as primary/secondary buttons, text input fields, checkboxes, combo boxes, and table selection highlights use a 4px corner radius.
- **Overlay & Card Geometry (8px):** Content containers, elevated cards, flyout menus, and modal dialog windows use an 8px radius (`rounded-lg`).
- **Pill Badges & Toggles (Full / Circular):** Toggle switch thumbs, numeric count badges, and presence indicators maintain circular or fully rounded capsules.
- **Window Frame:** The outer host window features standard native rounded corners (8px) when windowed, collapsing to 0px when fully maximized.

## Components

### Title Bar & Window Controls
- **Height & Layout:** Fixed 40px height with drag region.
- **Controls Group:** Positioned at the top-right corner containing Minimize, Maximize/Restore, and Close buttons.
  - Sizing: Width 46px, height 32px (Close button fills 46px × 40px flush).
  - Hover / Active: Standard buttons hover with `rgba(0, 0, 0, 0.05)`; Close button turns `#C42B1C` with white icon fill on hover.

### Buttons
- **Standard Button:** Background `rgba(255, 255, 255, 0.7)` with translucent stroke (`rgba(0, 0, 0, 0.06)`), 4px border radius, resting shadow. Hover increases surface opacity to solid white; pressed state translates 1px down.
- **Accent Button:** Solid `#005FB8` fill with crisp white text. Hover state brightens to `#116EB8`; pressed state darkens to `#004E98`. Includes an inset 1px border highlight (`rgba(255, 255, 255, 0.2)`).

### Input Fields & Controls
- **Text Inputs:** Height 32px, 4px corner radius, background `rgba(255, 255, 255, 0.7)`. On focus, shows a 2px accent-colored underline border at the bottom edge.
- **Checkboxes & Radios:** 18px bounds with a 1px border. Checked state fills with accent color `#005FB8` with an animated check glyph.
- **Toggle Switch:** Capsule track (40px × 20px) with sliding 12px circular handle. Track fills with accent color when turned on.

### Cards & Content Surfaces
- **Card Surface:** 8px border radius, flat white fill (`#FFFFFF`), surrounded by a 1px translucent perimeter border (`rgba(0, 0, 0, 0.06)`). Inner padding is configured to 16px (`space-lg`).
- **Interactive Cards:** Hover introduces a 2px elevation lift and subtle shadow expansion with continuous 150ms cubic-bezier transition.

### Chips & Tags
- Height 26px, 4px or pill rounded radius, neutral soft background (`rgba(0, 0, 0, 0.04)`), optional dismiss icon (12px glyph) with 4px hit margin.

### Context Menus & Flyouts
- Acrylic backdrop filter (blur 24px), 8px border radius, 4px inner padding. Menu items have 4px radius, 28px height, and render Fluent-style monochrome icons on the left with keyboard accelerators flush right.