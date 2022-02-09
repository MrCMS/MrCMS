
export function setupRegistrationsAdminReport(){

        $("[data-total-registrations]").each((index, element) => {
            const $el = $(element);
            const data = $el.data('total-registrations');

            const config = {
                type: 'bar',
                data: {
                    labels: data.map(x=>x.Date),
                    datasets: [{
                        label: 'Number of registrations',
                        backgroundColor: '#007bff',
                        borderColor: '#007bff',
                        data: data.map(x=>x.Count)
                    }],                    
                },
                options: {
                    title: {
                        display: true,
                        text: 'User Registrations'
                    },
                    tooltips: {
                        mode: 'index',
                        intersect: false,
                    },
                    hover: {
                        mode: 'nearest',
                        intersect: true
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            scaleLabel: {
                                display: true,
                                labelString: 'Date'
                            }
                        }],
                        yAxes: [{
                            display: true,
                            scaleLabel: {
                                display: true,
                                labelString: 'Count'
                            },
                            ticks: {
                                beginAtZero: true,
                                precision: 0,
                            }
                        }]
                    }
                }
            };

            const ctx = element.getContext('2d');
            const chart = new Chart(ctx, config);
        });
    
}