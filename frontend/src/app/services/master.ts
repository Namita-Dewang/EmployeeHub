import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { retry } from 'rxjs';
import { DepartmentModel, DesignationModel } from '../models/Department.model';
import { EmployeeModel } from '../models/Employee.model';

@Injectable({
  providedIn: 'root',
})
export class Master {
  readonly apiUrl = 'http://localhost:5279/api';
  private readonly http = inject(HttpClient);

  getAllDept() {
    return this.withStartupRetry<DepartmentModel[]>(`${this.apiUrl}/DepartmentMaster/GetAllDepartments`);
  }

  getAllDesignations() {
    return this.withStartupRetry<DesignationModel[]>(`${this.apiUrl}/DesignationMaster/GetDesignations`);
  }

  getEmployee(id: number) {
    return this.withStartupRetry<EmployeeModel>(`${this.apiUrl}/EmployeeMaster/GetEmployee/${id}`);
  }

  getEmployees(pageNumber = 1, pageSize = 10) {
    return this.withStartupRetry<EmployeePage>(
      `${this.apiUrl}/EmployeeMaster/FilterEmployees?pageNumber=${pageNumber}&pageSize=${pageSize}&sortBy=name&sortOrder=asc`,
    );
  }

  private withStartupRetry<T>(url: string) {
    return this.http.get<T>(url).pipe(
      retry({ count: 2, delay: 500 }),
    );
  }
}

export interface EmployeePage {
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  data: EmployeeModel[];
}
