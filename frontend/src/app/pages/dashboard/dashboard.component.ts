import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/auth.service';
import { UsersService } from '../../core/users.service';
import { ProiecteService, type ProiectDto } from '../../core/proiecte.service';
import { SantierService, type SantierDto } from '../../core/santier.service';
import { EchipeService, type EchipaDto } from '../../core/echipe.service';
import { AngajatiService } from '../../core/angajati.service';
import { LucrariService, type LucrareDto } from '../../core/lucrari.service';
import { ZardCardComponent } from '@/shared/components/card';
import { ZardLoaderComponent } from '@/shared/components/loader';
import { ZardAlertComponent } from '@/shared/components/alert';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ZardCardComponent, ZardLoaderComponent, ZardAlertComponent],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent {
  readonly auth = inject(AuthService);
  private usersService = inject(UsersService);
  private proiecteService = inject(ProiecteService);
  private santierService = inject(SantierService);
  private echipeService = inject(EchipeService);
  private angajatiService = inject(AngajatiService);
  private lucrariService = inject(LucrariService);

  loading = signal(true);
  error = signal<string | null>(null);

  usersCount = signal<number | null>(null);
  proiecte = signal<ProiectDto[]>([]);
  santiere = signal<SantierDto[]>([]);
  echipe = signal<EchipaDto[]>([]);
  angajatiCount = signal(0);
  lucrari = signal<LucrareDto[]>([]);

  // Statistici calculate
  proiecteActive = computed(() => this.proiecte().filter((p) => p.stare === 'Activ').length);
  lucrariPlanificate = computed(() => this.lucrari().filter((l) => l.stare === 'Planificat').length);
  lucrariInLucru = computed(() => this.lucrari().filter((l) => l.stare === 'În lucru').length);
  lucrariFinalizate = computed(() => this.lucrari().filter((l) => l.stare === 'Finalizat').length);
  totalLucrari = computed(() => this.lucrari().length);
  procentFinalizat = computed(() => {
    const total = this.totalLucrari();
    if (total === 0) return 0;
    return Math.round((this.lucrariFinalizate() / total) * 100);
  });

  // Lucrări cu deadline depășit
  lucrariDeadlineDepasit = computed(() => {
    const now = new Date();
    return this.lucrari()
      .filter((l) => {
        const termen = new Date(l.termen);
        return l.stare !== 'Finalizat' && termen < now;
      })
      .sort((a, b) => new Date(a.termen).getTime() - new Date(b.termen).getTime())
      .slice(0, 5);
  });

  // Lucrări cu deadline în următoarele 7 zile
  lucrariDeadlineAproape = computed(() => {
    const now = new Date();
    const in7Days = new Date();
    in7Days.setDate(now.getDate() + 7);
    return this.lucrari()
      .filter((l) => {
        const termen = new Date(l.termen);
        return l.stare !== 'Finalizat' && termen >= now && termen <= in7Days;
      })
      .sort((a, b) => new Date(a.termen).getTime() - new Date(b.termen).getTime())
      .slice(0, 5);
  });

  constructor() {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    const calls: any = {
      proiecte: this.proiecteService.getAll(),
      santiere: this.santierService.getAll(),
      echipe: this.echipeService.getAll(),
      angajati: this.angajatiService.getAll(),
      lucrari: this.lucrariService.getAll(),
    };

    if (this.auth.isAdmin()) {
      calls.users = this.usersService.getAll();
    }

    forkJoin(calls).subscribe({
      next: (data: any) => {
        this.proiecte.set(data.proiecte ?? []);
        this.santiere.set(data.santiere ?? []);
        this.echipe.set(data.echipe ?? []);
        this.angajatiCount.set((data.angajati ?? []).length);
        this.lucrari.set(data.lucrari ?? []);
        if (data.users) {
          this.usersCount.set(data.users.length);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Eroare la încărcarea datelor.');
      },
    });
  }

  getDaysUntil(dateStr: string): number {
    const now = new Date();
    const target = new Date(dateStr);
    const diff = target.getTime() - now.getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  }
}
