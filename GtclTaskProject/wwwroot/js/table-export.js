//// Table Export Helper Functions
//const tableExport = {
//    // Filter table data based on search and pagination
//    getFilteredTableData: function (tableId) {
//        const table = document.getElementById(tableId);
//        if (!table) return [];

//        const headers = [];
//        const rows = [];

//        // Get headers
//        const headerRow = table.querySelector('thead tr');
//        if (headerRow) {
//            headerRow.querySelectorAll('th').forEach(th => {
//                headers.push(th.innerText.trim());
//            });
//        }

//        // Get visible rows (account for DataTables if used)
//        const dataTableObj = $(tableId).DataTable();
//        let visibleRows;

//        if (dataTableObj) {
//            // Using DataTables - get filtered/sorted rows
//            visibleRows = dataTableObj.rows({ search: 'applied' }).nodes();
//        } else {
//            // Standard table
//            visibleRows = table.querySelectorAll('tbody tr');
//        }

//        // Process rows
//        visibleRows.forEach(row => {
//            const rowData = [];
//            row.querySelectorAll('td').forEach(cell => {
//                // Handle cells with images
//                if (cell.querySelector('img')) {
//                    rowData.push('[Image]');
//                } else {
//                    rowData.push(cell.innerText.trim());
//                }
//            });
//            rows.push(rowData);
//        });

//        return {
//            headers: headers,
//            rows: rows
//        };
//    },

//    // Create and download a file
//    downloadFile: function (content, fileName, mimeType) {
//        const blob = new Blob([content], { type: mimeType });
//        const link = document.createElement('a');
//        link.href = URL.createObjectURL(blob);
//        link.download = fileName;
//        document.body.appendChild(link);
//        link.click();
//        document.body.removeChild(link);
//    },

//    // Export to CSV client-side
//    exportToCsv: function (tableId, fileName) {
//        const data = this.getFilteredTableData(tableId);
//        let csv = '';

//        // Add headers
//        csv += data.headers.map(header => `"${header}"`).join(',') + '\n';

//        // Add rows
//        data.rows.forEach(row => {
//            csv += row.map(cell => `"${cell}"`).join(',') + '\n';
//        });

//        this.downloadFile(csv, fileName || 'export.csv', 'text/csv');
//    },

//    // Export to HTML for printing
//    printTable: function (tableId, title) {
//        const data = this.getFilteredTableData(tableId);

//        // Create a new window
//        const printWindow = window.open('', '_blank');
//        printWindow.document.write(`
//            <!DOCTYPE html>
//            <html>
//            <head>
//                <title>${title || 'Table Export'}</title>
//                <style>
//                    body { font-family: Arial, sans-serif; }
//                    table { border-collapse: collapse; width: 100%; margin-top: 20px; }
//                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
//                    th { background-color: #f2f2f2; }
//                    h1 { text-align: center; }
//                    .date { text-align: right; font-style: italic; margin-bottom: 20px; }
//                    @media print {
//                        button { display: none; }
//                        body { margin: 0; padding: 15mm; }
//                    }
//                </style>
//            </head>
//            <body>
//                <button onclick="window.print()" style="padding: 5px 10px; margin: 10px 0;">Print</button>
//                <h1>${title || 'Table Export'}</h1>
//                <p class="date">Generated on: ${new Date().toLocaleString()}</p>
//                <table>
//                    <thead>
//                        <tr>
//                            ${data.headers.map(header => `<th>${header}</th>`).join('')}
//                        </tr>
//                    </thead>
//                    <tbody>
//                        ${data.rows.map(row =>
//            `<tr>${row.map(cell => `<td>${cell}</td>`).join('')}</tr>`
//        ).join('')}
//                    </tbody>
//                </table>
//            </body>
//            </html>
//        `);
//        printWindow.document.close();
//    }
//};

//// Initialize table export functionality
//document.addEventListener('DOMContentLoaded', function () {
//    // Add client-side export handlers if needed
//    const clientExportButtons = document.querySelectorAll('[data-export-table]');

//    clientExportButtons.forEach(button => {
//        const tableId = button.getAttribute('data-export-table');
//        const exportType = button.getAttribute('data-export-type');
//        const fileName = button.getAttribute('data-export-filename');

//        button.addEventListener('click', function (e) {
//            e.preventDefault();

//            switch (exportType) {
//                case 'csv':
//                    tableExport.exportToCsv(tableId, fileName || `export_${new Date().toISOString().slice(0, 10)}.csv`);
//                    break;
//                case 'print':
//                    tableExport.printTable(tableId, 'Table Data');
//                    break;
//                // Other client-side exports would go here
//            }
//        });
//    });
//});

// Table Export Helper Functions
const tableExport = {
    // Filter table data based on search and pagination
    getFilteredTableData: function (tableId) {
        const table = document.getElementById(tableId);
        if (!table) return [];

        const headers = [];
        const rows = [];

        // Get headers
        const headerRow = table.querySelector('thead tr');
        if (headerRow) {
            headerRow.querySelectorAll('th').forEach(th => {
                headers.push(th.innerText.trim());
            });
        }

        // Get visible rows (account for DataTables if used)
        const dataTableObj = $(tableId).DataTable();
        let visibleRows;

        if (dataTableObj) {
            // Using DataTables - get filtered/sorted rows
            visibleRows = dataTableObj.rows({ search: 'applied' }).nodes();
        } else {
            // Standard table
            visibleRows = table.querySelectorAll('tbody tr');
        }

        // Process rows
        visibleRows.forEach(row => {
            const rowData = [];
            row.querySelectorAll('td').forEach((cell, index) => {
                // Skip the Actions column (last column)
                if (index < headers.length - 1) {
                    // Handle cells with images
                    if (cell.querySelector('img')) {
                        rowData.push('[Image]');
                    } else {
                        rowData.push(cell.innerText.trim());
                    }
                }
            });
            rows.push(rowData);
        });

        // Remove the "Actions" header if it exists
        if (headers.length > 0 && headers[headers.length - 1] === "Actions") {
            headers.pop();
        }

        return {
            headers: headers,
            rows: rows
        };
    },

    // Create and download a file
    downloadFile: function (content, fileName, mimeType) {
        const blob = new Blob([content], { type: mimeType });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },

    // Export to CSV client-side
    exportToCsv: function (tableId, fileName) {
        const data = this.getFilteredTableData(tableId);
        let csv = '';

        // Add headers
        csv += data.headers.map(header => `"${header}"`).join(',') + '\n';

        // Add rows
        data.rows.forEach(row => {
            csv += row.map(cell => `"${cell}"`).join(',') + '\n';
        });

        this.downloadFile(csv, fileName || 'export.csv', 'text/csv');
    },

    // Export to HTML for printing/PDF
    printTable: function (tableId, title) {
        const data = this.getFilteredTableData(tableId);

        // Create a new window
        const printWindow = window.open('', '_blank');
        printWindow.document.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>${title || 'Table Export'}</title>
                <style>
                    body { font-family: Arial, sans-serif; }
                    table { border-collapse: collapse; width: 100%; margin-top: 20px; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    h1 { text-align: center; }
                    .date { text-align: right; font-style: italic; margin-bottom: 20px; }
                    @media print {
                        .no-print { display: none !important; }
                        body { margin: 0; padding: 15mm; }
                    }
                </style>
                <script>
                    window.onload = function() {
                        setTimeout(function() {
                            window.print();
                            // Optional: Close the window after printing
                            // setTimeout(function() { window.close(); }, 500);
                        }, 500);
                    }
                </script>
            </head>
            <body>
                <div class="no-print">
                    <button onclick="window.print()" style="padding: 5px 10px; margin: 10px 0;">Print</button>
                    <button onclick="window.close()" style="padding: 5px 10px; margin: 10px 0;">Close</button>
                </div>
                <h1>${title || 'Table Export'}</h1>
                <p class="date">Generated on: ${new Date().toLocaleString()}</p>
                <table>
                    <thead>
                        <tr>
                            ${data.headers.map(header => `<th>${header}</th>`).join('')}
                        </tr>
                    </thead>
                    <tbody>
                        ${data.rows.map(row =>
            `<tr>${row.map(cell => `<td>${cell}</td>`).join('')}</tr>`
        ).join('')}
                    </tbody>
                </table>
            </body>
            </html>
        `);
        printWindow.document.close();
    }
};

// Initialize table export functionality
document.addEventListener('DOMContentLoaded', function () {
    // Add client-side export handlers if needed
    const clientExportButtons = document.querySelectorAll('[data-export-table]');

    clientExportButtons.forEach(button => {
        const tableId = button.getAttribute('data-export-table');
        const exportType = button.getAttribute('data-export-type');
        const fileName = button.getAttribute('data-export-filename');

        button.addEventListener('click', function (e) {
            e.preventDefault();

            switch (exportType) {
                case 'csv':
                    tableExport.exportToCsv(tableId, fileName || `export_${new Date().toISOString().slice(0, 10)}.csv`);
                    break;
                case 'print':
                case 'pdf':
                    tableExport.printTable(tableId, 'Customer Data');
                    break;
                // Other client-side exports would go here
            }
        });
    });
});