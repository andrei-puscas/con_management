import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="rounded-lg bg-white p-6 shadow">
      <h2 class="text-lg font-medium text-gray-900">Dashboard</h2>
      <p class="mt-2 text-gray-600">Bine ai venit. Pagina de dashboard va fi populată în fazele următoare.</p>
    </div>
  `,
  styles: [],
})
export class DashboardComponent {}
