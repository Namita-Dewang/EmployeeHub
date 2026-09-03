import { Injectable, signal } from '@angular/core';

export type Theme = 'dark' | 'light';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly STORAGE_KEY = 'ph-theme';

  /** Reactive signal so any component can read the current theme */
  readonly theme = signal<Theme>(this.loadTheme());

  constructor() {
    this.applyTheme(this.theme());
  }

  toggle(): void {
    const next: Theme = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(next);
    this.applyTheme(next);
    localStorage.setItem(this.STORAGE_KEY, next);
  }

  private loadTheme(): Theme {
    const stored = localStorage.getItem(this.STORAGE_KEY) as Theme | null;
    if (stored === 'dark' || stored === 'light') return stored;
    // Default to dark if no preference
    return 'dark';
  }

  private applyTheme(theme: Theme): void {
    document.documentElement.setAttribute('data-theme', theme);
  }
}
