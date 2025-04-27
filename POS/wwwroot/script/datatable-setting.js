$('document').ready(function () {
    $('.data-table').DataTable({
        "bDestroy": true,
        "sScrollX": "100%",
        "sScrollXInner": "100%",
        "bScrollCollapse": true,
        columnDefs: [{
            targets: "datatable-nosort",
            orderable: false,
        }],
        "oLanguage": {
            "sSearchPlaceholder": "Tìm Kiếm",
            "oPaginate": {
                "sNext": '<i class="ion-chevron-right"></i>',
                "sPrevious": '<i class="ion-chevron-left"></i>'
            },
            "sUrl": "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        }
    });

    var tableCustom = $('.data-table-export').DataTable({
        "bDestroy": true,
        "sScrollX": "100%",
        "sScrollXInner": "100%",
        "bScrollCollapse": true,
        columnDefs: [{
            targets: "datatable-nosort",
            orderable: false,
        }
        ],
        "oLanguage": {
            "sSearchPlaceholder": "Tìm Kiếm",
            "oPaginate": {
                "sNext": '<i class="ion-chevron-right"></i>',
                "sPrevious": '<i class="ion-chevron-left"></i>'
            },
            "sUrl": "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        },
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'csvHtml5',
                text: '<i class="ion-ios-download"></i> CSV',
                className: 'btn btn-success'
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="ion-ios-download"></i> PDF',
                className: 'btn btn-danger'
            }
        ],
        "drawCallback": function (settings) {
            var api = this.api();
            var startIndex = api.page.info().start;
            var firstHeaderTitle = api.column(0).header().textContent;
            if (firstHeaderTitle == "#") {
                api.column(0, { page: 'current' }).nodes().each(function (cell, i) {
                    cell.innerHTML = startIndex + i + 1;
                });
            }
        },
        initComplete: function () {
            var nameColumns = tableCustom.columns().header().toArray().map(function (header) {
                return $(header).text();
            });

            var stringHtml = '<option value="-1">Tất cả</option>';
            var count = 0;
            nameColumns.forEach((element) => {
                if (element != '#' && element != 'Tùy Chọn') {
                    stringHtml += `<option value="${count}">${element}</option>`;
                    count++;
                }
            });

            var filterDiv = document.getElementById('DataTables_Table_0_filter');
            if (filterDiv != null) {
                filterDiv.innerHTML = `<div id="columnFilter" class="flex-box"> <label for="filterColumn">Lọc theo: </label> <select id="filterColumn" class="form-control form-control-sm" style="width: 209px;margin-left: 5px;"> ${stringHtml} </select> </div> <label class="flex-box" style="align-items: flex-start;"> Tìm: <input id="filterInput" type="search" class="form-control form-control-sm" style="margin-left: 5px;" placeholder="Tìm Kiếm" aria-controls="DataTables_Table_0"> </label>`;
            }

            $('#filterColumn').on('change', function () {
                var filterValue = $(this).val();
                var searchTerm = $('#filterInput').val();
                tableCustom.search('', true, false).columns().search('');
                if (filterValue === '-1') {
                    tableCustom.search(searchTerm).draw();
                } else {
                    var columnIndex = filterValue;
                    tableCustom.column(filterValue).search(searchTerm).draw();
                }
            });

            $('#filterInput').on('input', function () {
                var searchTerm = $(this).val();
                var filterValue = $('#filterColumn').val();
                tableCustom.search('', true, false).columns().search('');
                if (filterValue === '-1') {
                    tableCustom.search(searchTerm).draw();
                } else {
                    var columnIndex = filterValue;
                    tableCustom.column(columnIndex).search(searchTerm).draw();
                }
            });

            $('th.datatable-nosort').removeClass('sorting sorting_asc sorting_desc');
        }
    });

    var tableGroup = $('.data-table-group').DataTable({
        "bDestroy": true,
        "sScrollX": "100%",
        "sScrollXInner": "100%",
        "bScrollCollapse": true,
        "ordering": false,
        "order": [],
        columnDefs: [
            {
                targets: "datatable-nosort",
                orderable: false,
            },
            {
                targets: 1,
                visible: false
            },
            {
                "orderable": true,
                "targets": 0
            },
            { "orderable": false, "targets": -1 }
        ],
        "oLanguage": {
            "sSearchPlaceholder": "Tìm Kiếm",
            "oPaginate": {
                "sNext": '<i class="ion-chevron-right"></i>',
                "sPrevious": '<i class="ion-chevron-left"></i>'
            },
            "sUrl": "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Vietnamese.json"
        },
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'csvHtml5',
                text: '<i class="ion-ios-download"></i> CSV',
                className: 'btn btn-success'
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="ion-ios-download"></i> PDF',
                className: 'btn btn-danger'
            }
        ],
        "order": [[1, 'asc']],
        "drawCallback": function (settings) { //group by column 2 
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;
            var startIndex = api.page.info().start;
            var columnHeaderTitle = api.column(1).header().textContent;

            api.column(1, { page: 'current' })
                .data()
                .each(function (group, i) {
                    if (last !== group) {
                        $(rows)
                            .eq(i)
                            .before(
                                '<tr class="group"><td style="display: flex; border: none; font-weight: 600;font-size: medium;" colspan="5">' +
                                group +
                                '</td></tr>'
                            );

                        last = group;
                    }
                });

            var firstHeaderTitle = api.column(0).header().textContent;
            if (firstHeaderTitle == "#") {
                api.column(0, { page: 'current' }).nodes().each(function (cell, i) {
                    cell.innerHTML = startIndex + i + 1;
                });
            }
        },
        initComplete: function () {
            var nameColumns = tableGroup.columns().header().toArray().map(function (header) {
                return $(header).text();
            });


            var stringHtml = '<option value="0">Tất cả</option>';
            var count = 1;
            nameColumns.forEach((element) => {
                if (element != '#' && element != 'Tùy Chọn') {
                    stringHtml += `<option value="${count}">${element}</option>`;
                    count++;
                }
            });

            var filterDiv = document.getElementById('DataTables_Table_0_filter');
            if (filterDiv != null) {
                filterDiv.innerHTML = `<div id="columnFilter" class="flex-box"> <label for="filterColumn">Lọc theo: </label> <select id="filterColumn" class="form-control form-control-sm" style="width: 209px;margin-left: 5px;"> ${stringHtml} </select> </div> <label class="flex-box" style="align-items: flex-start;"> Tìm: <input id="filterInput" type="search" class="form-control form-control-sm" style="margin-left: 5px;" placeholder="Tìm Kiếm" aria-controls="DataTables_Table_0"> </label>`;
            }

            $('#filterColumn').on('change', function () {
                var filterValue = $(this).val();
                var searchTerm = $('#filterInput').val();
                tableGroup.search('', true, false).columns().search('');
                if (filterValue === '0') {
                    tableGroup.search(searchTerm).draw();
                } else {
                    var columnIndex = filterValue;
                    tableGroup.column(filterValue).search(searchTerm).draw();
                }
            });

            $('#filterInput').on('input', function () {
                var searchTerm = $(this).val();
                var filterValue = $('#filterColumn').val();
                tableGroup.search('', true, false).columns().search('');
                if (filterValue === '0') {
                    tableGroup.search(searchTerm).draw();
                } else {
                    var columnIndex = filterValue - 1;
                    tableGroup.column(columnIndex).search(searchTerm).draw();
                }
            });

            $('th.datatable-nosort').removeClass('sorting sorting_asc sorting_desc');
        }
    });

    var table = $('.select-row').DataTable();
    $('.select-row tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    var multipletable = $('.multiple-select-row').DataTable();
    $('.multiple-select-row tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });
    var table = $('.checkbox-datatable').DataTable({
        'scrollCollapse': true,
        'autoWidth': false,
        'responsive': true,
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
        "language": {
            "info": "_START_-_END_ of _TOTAL_ entries",
            searchPlaceholder: "Search",
            paginate: {
                next: '<i class="ion-chevron-right"></i>',
                previous: '<i class="ion-chevron-left"></i>'
            }
        },
        'columnDefs': [{
            'targets': 0,
            'searchable': false,
            'orderable': false,
            'className': 'dt-body-center',
            'render': function (data, type, full, meta) {
                return '<div class="dt-checkbox"><input type="checkbox" name="id[]" value="' + $('<div/>').text(data).html() + '"><span class="dt-checkbox-label"></span></div>';
            }
        }],
        'order': [[1, 'asc']]
    });

    $('#example-select-all').on('click', function () {
        var rows = table.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
    });

    $('.checkbox-datatable tbody').on('change', 'input[type="checkbox"]', function () {
        if (!this.checked) {
            var el = $('#example-select-all').get(0);
            if (el && el.checked && ('indeterminate' in el)) {
                el.indeterminate = true;
            }
        }
    });
});
