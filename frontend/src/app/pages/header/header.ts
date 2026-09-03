import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  isSidebarOpen = false;
  readonly themeService = inject(ThemeService);

  constructor(private readonly router: Router) {}

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar() {
    this.isSidebarOpen = false;
  }

  logout() {
    localStorage.removeItem('empLoginUser');
    this.closeSidebar();
    this.router.navigate(['/login']);
  }
}
