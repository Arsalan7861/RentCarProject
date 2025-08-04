/* eslint-disable @typescript-eslint/no-inferrable-types */
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
  ViewEncapsulation,
} from '@angular/core';
import { BreadcrumbService } from '../../services/breadcrumb';
import Blank from '../../components/blank/blank';
import { httpResource } from '@angular/common/http';
import { Result } from '../../../../../libraries/shared/src/lib/models/result.model';
import Loading from '../../components/loading/loading';
import { TrCurrencyPipe } from 'tr-currency';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';

Chart.register(...registerables);

interface ChartData {
  data: { date: string; total: number }[];
  borderColor: string[];
}

interface ReservationWeeklyState {
  Date: string;
  TotalReservations: number;
}

@Component({
  imports: [Blank, Loading, TrCurrencyPipe],
  templateUrl: './dashboard.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Dashboard implements OnInit, AfterViewInit {
  readonly activeReservationCountResult = httpResource<Result<number>>(
    () => '/rent/dashboard/active-reservation-count'
  );
  readonly activeReservationCount = computed(
    () => this.activeReservationCountResult.value()?.data ?? 0
  );
  readonly activeReservationCountLoading = computed(() =>
    this.activeReservationCountResult.isLoading()
  );

  readonly vehicleCountResult = httpResource<Result<number>>(
    () => '/rent/dashboard/vehicle-count'
  );
  readonly vehicleCount = computed(
    () => this.vehicleCountResult.value()?.data ?? 0
  );
  readonly vehicleCountLoading = computed(() =>
    this.vehicleCountResult.isLoading()
  );

  readonly monthlyIncomeResult = httpResource<Result<number>>(
    () => '/rent/dashboard/monthly-income'
  );
  readonly monthlyIncome = computed(
    () => this.monthlyIncomeResult.value()?.data ?? 0
  );
  readonly monthlyIncomeLoading = computed(() =>
    this.monthlyIncomeResult.isLoading()
  );
  readonly currentMonth: number = (new Date().getMonth() + 1) as number;
  readonly currentMonthName = signal('');

  readonly customerCountResult = httpResource<Result<number>>(
    () => '/rent/dashboard/customer-count'
  );
  readonly customerCount = computed(
    () => this.customerCountResult.value()?.data ?? 0
  );
  readonly customerCountLoading = computed(() =>
    this.customerCountResult.isLoading()
  );

  readonly reservationWeeklyStateResult = httpResource<
    Result<ReservationWeeklyState[]>
  >(() => '/rent/dashboard/weekly-reservation-state');
  readonly reservationWeeklyState = computed(
    () => this.reservationWeeklyStateResult.value()?.data ?? []
  );
  readonly reservationWeeklyStateLoading = computed(() =>
    this.reservationWeeklyStateResult.isLoading()
  );

  readonly revenueChartCanvas =
    viewChild.required<ElementRef<HTMLCanvasElement>>('revenueChartCanvas');
  readonly reservationWeeklyStateChartCanvas = viewChild.required<
    ElementRef<HTMLCanvasElement>
  >('reservationWeeklyStateChartCanvas');

  readonly revenueChart = signal<Chart | null>(null);
  readonly reservationWeeklyStateChart = signal<Chart | null>(null);

  readonly #breadcrumb = inject(BreadcrumbService);

  readonly res1 = signal<ChartData>({
    data: [
      { date: '01.07.2025', total: 150.5 },
      { date: '02.07.2025', total: 200.75 },
      { date: '03.07.2025', total: 175.25 },
      { date: '04.07.2025', total: 220.0 },
      { date: '05.07.2025', total: 180.8 },
      { date: '06.07.2025', total: 300.45 },
      { date: '07.07.2025', total: 250.3 },
    ],
    borderColor: [
      'rgba(54, 162, 235, 1)',
      'rgba(255, 99, 132, 1)',
      'rgba(255, 205, 86, 1)',
      'rgba(75, 192, 192, 1)',
      'rgba(153, 102, 255, 1)',
      'rgba(255, 159, 64, 1)',
      'rgba(199, 199, 199, 1)',
    ],
  });
  readonly res2 = signal<ChartData>({
    data: [
      { date: '01.07.2025', total: 5 },
      { date: '02.07.2025', total: 10 },
      { date: '03.07.2025', total: 20 },
      { date: '04.07.2025', total: 30 },
      { date: '05.07.2025', total: 4 },
      { date: '06.07.2025', total: 50 },
      { date: '07.07.2025', total: 8 },
    ],
    borderColor: [
      'rgba(54, 162, 235, 1)',
      'rgba(255, 99, 132, 1)',
      'rgba(255, 205, 86, 1)',
      'rgba(75, 192, 192, 1)',
      'rgba(153, 102, 255, 1)',
      'rgba(255, 159, 64, 1)',
      'rgba(199, 199, 199, 1)',
    ],
  });

  ngOnInit(): void {
    this.#breadcrumb.setDashboard();
    this.setCurrentMonthName();
  }

  ngAfterViewInit(): void {
    this.createChart(
      this.revenueChart(),
      this.revenueChartCanvas(),
      'bar',
      this.res1(),
      'Günlük Rezervasyon Sayısı',
      'Haftalık Rezervasyon Dağılımı',
      ''
    );

    this.createChart(
      this.reservationWeeklyStateChart(),
      this.reservationWeeklyStateChartCanvas(),
      'pie',
      this.res2(),
      'Günlük Rezervasyon',
      'Haftalık Rezervasyon Dağılımı',
      ''
    );
  }

  createChart(
    chart: Chart | null,
    canvas: any,
    type: ChartType,
    res: ChartData,
    label: string,
    text: string,
    symbol: string = ' ₺'
  ) {
    if (chart) {
      chart.destroy();
    }

    const ctx = canvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const config: ChartConfiguration = {
      type: type,
      data: {
        labels: res.data.map((item) => item.date),
        datasets: [
          {
            label: label,
            data: res.data.map((item) => item.total),
            backgroundColor: res.borderColor,
            borderColor: res.borderColor,
            borderWidth: 2,
            fill: type === 'line' ? false : true,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          title: {
            display: true,
            text: text,
          },
          legend: {
            display: true,
            position: 'top',
          },
        },
        scales:
          type !== 'doughnut'
            ? {
                y: {
                  beginAtZero: true,
                  ticks: {
                    callback: function (value) {
                      return value + symbol;
                    },
                  },
                },
              }
            : {},
      },
    };

    chart = new Chart(ctx, config);
  }

  private setCurrentMonthName(): void {
    switch (this.currentMonth) {
      case 1:
        this.currentMonthName.set('Ocak');
        break;
      case 2:
        this.currentMonthName.set('Şubat');
        break;
      case 3:
        this.currentMonthName.set('Mart');
        break;
      case 4:
        this.currentMonthName.set('Nisan');
        break;
      case 5:
        this.currentMonthName.set('Mayıs');
        break;
      case 6:
        this.currentMonthName.set('Haziran');
        break;
      case 7:
        this.currentMonthName.set('Temmuz');
        break;
      case 8:
        this.currentMonthName.set('Ağustos');
        break;
      case 9:
        this.currentMonthName.set('Eylül');
        break;
      case 10:
        this.currentMonthName.set('Ekim');
        break;
      case 11:
        this.currentMonthName.set('Kasım');
        break;
      case 12:
        this.currentMonthName.set('Aralık');
        break;
    }
  }
}
