import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Master } from '../../services/master';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  private readonly master = inject(Master);
  employeeCount = 0;
  departmentCount = 0;
  designationCount = 0;
  isLoading = true;
  errorMessage = '';

  constructor() {
    this.master.getEmployees(1, 1).subscribe({
      next: (page) => { this.employeeCount = page.totalRecords; this.isLoading = false; },
      error: () => { this.isLoading = false; this.showError(); },
    });
    this.master.getAllDept().subscribe({
      next: (items) => this.departmentCount = items.length,
      error: () => this.showError(),
    });
    this.master.getAllDesignations().subscribe({
      next: (items) => this.designationCount = items.length,
      error: () => this.showError(),
    });
  }

  private showError() {
    this.errorMessage = 'Some dashboard data could not be loaded. Please refresh the page.';
  }
}
