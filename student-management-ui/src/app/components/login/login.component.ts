import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;
  showPassword = false;

  constructor(private authService: AuthService, private router: Router) {
    if (this.authService.isLoggedIn()) this.router.navigate(['/students']);
  }

  login(): void {
    if (!this.username || !this.password) {
      this.errorMessage = 'Please enter username and password.';
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login({ username: this.username, password: this.password }).subscribe({
      next: res => {
        this.isLoading = false;
        if (res.success) {
          this.router.navigate(['/students']);
        } else {
          this.errorMessage = res.message;
        }
      },
      error: err => {
        this.isLoading = false;
        this.errorMessage = err?.error?.message || 'Invalid username or password.';
      }
    });
  }

  fillDemo(role: 'admin' | 'user'): void {
    this.username = role;
    this.password = role === 'admin' ? 'Admin@123' : 'User@123';
  }
}
