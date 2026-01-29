import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="min-h-screen bg-gray-50">
      <header class="bg-white shadow">
        <div class="mx-auto max-w-7xl px-4 py-4">
          <h1 class="text-xl font-semibold text-gray-800">ConManagement</h1>
        </div>
      </header>
      <main class="mx-auto max-w-7xl px-4 py-6">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [],
})
export class AppComponent {
  title = 'ConManagement';
}
