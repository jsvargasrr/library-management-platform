import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastService } from '../../../core/notifications/toast.service';
import { AuthorsApi } from '../authors.api';
import { Author } from '../authors.models';

function isoDateOrNull(value: unknown): string | null {
  const s = (value ?? '').toString().trim();
  return s.length ? s : null;
}

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './author-form.page.html',
  styleUrl: './author-form.page.scss'
})
export class AuthorFormPage {
  private readonly api = inject(AuthorsApi);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly author = signal<Author | null>(null);

  readonly isEdit = computed(() => !!this.route.snapshot.paramMap.get('id'));

  readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(320)]],
    birthDate: [''],
    city: ['', [Validators.maxLength(120)]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.loading.set(true);
    this.api
      .get(id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: a => {
          this.author.set(a);
          this.form.patchValue({
            fullName: a.fullName,
            email: a.email,
            birthDate: a.birthDate ?? '',
            city: a.city ?? ''
          });
        }
      });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.error('Revisa los campos marcados.');
      return;
    }

    const payload = {
      fullName: this.form.controls.fullName.value.trim(),
      email: this.form.controls.email.value.trim(),
      birthDate: isoDateOrNull(this.form.controls.birthDate.value),
      city: isoDateOrNull(this.form.controls.city.value)
    };

    this.loading.set(true);

    const id = this.route.snapshot.paramMap.get('id');
    const obs = id ? this.api.update(id, payload) : this.api.create(payload);

    obs.pipe(finalize(() => this.loading.set(false))).subscribe({
      next: saved => {
        this.toast.success(id ? 'Autor actualizado.' : 'Autor creado.');
        this.router.navigate(['/authors', saved.id]);
      }
    });
  }
}

