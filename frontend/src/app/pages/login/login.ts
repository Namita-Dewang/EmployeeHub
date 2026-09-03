import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  loginobj: any = {
    email: '',
    contact: '',
  };

  http = inject(HttpClient);
  router = inject(Router);
  readonly themeService = inject(ThemeService);

  onLogin() {
    this.http.post('http://localhost:5279/api/EmployeeMaster/Login', this.loginobj).subscribe({
      next: (result: any) => {
        this.router.navigate(['/dashboard']);
        localStorage.setItem('empLoginUser', JSON.stringify(result));
      },
      error: () => {
        alert('Invalid email or contact');
      },
    });
  }
}
