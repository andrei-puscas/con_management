import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatBotService, type ChatMessage } from '../../../core/chat-bot.service';
import { ZardButtonComponent } from '@/shared/components/button';

@Component({
  selector: 'app-chat-bot-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, ZardButtonComponent],
  templateUrl: './chat-bot-panel.component.html',
})
export class ChatBotPanelComponent {
  private chat = inject(ChatBotService);

  open = signal(false);
  input = signal('');
  loading = signal(false);
  error = signal<string | null>(null);
  messages = signal<ChatMessage[]>([]);

  toggle(): void {
    this.open.update((v) => !v);
    this.error.set(null);
  }

  send(): void {
    const text = this.input().trim();
    if (!text || this.loading()) return;

    this.error.set(null);
    const userMsg: ChatMessage = { role: 'user', content: text };
    this.messages.update((m) => [...m, userMsg]);
    this.input.set('');
    this.loading.set(true);

    this.chat.send(this.messages()).subscribe({
      next: (reply) => {
        this.loading.set(false);
        this.messages.update((m) => [...m, { role: 'assistant', content: reply.content }]);
      },
      error: (err) => {
        this.loading.set(false);
        const msg =
          err?.error?.error ??
          (typeof err?.error === 'string' ? err.error : null) ??
          'Nu am putut obține răspunsul. Verifică cheia OpenAI pe server.';
        this.error.set(typeof msg === 'string' ? msg : 'Eroare la chat.');
      },
    });
  }
}
