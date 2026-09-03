import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Department } from './pages/department/department';
import { Designation } from './pages/designation/designation';
import { EmployeeForm } from './pages/employee-form/employee-form';
import { EmployeeList } from './pages/employee-list/employee-list';
import { Header } from './pages/header/header';
import { Login } from './pages/login/login';

export const routes: Routes = [
   {
      path: '',
      redirectTo: 'login',
      pathMatch: 'full'
   },
   {
      path: 'login',
      component: Login
   },
   {
    path: '',
    component: Header,
    children: [
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path:'new-employee',
        component: EmployeeForm
      },
      {
        path: 'department',
        component: Department
      },
      {
        path: 'designation',
        component: Designation
      },
      {
        path: 'employee-list',
        component: EmployeeList
      }
    ]
   }
];
