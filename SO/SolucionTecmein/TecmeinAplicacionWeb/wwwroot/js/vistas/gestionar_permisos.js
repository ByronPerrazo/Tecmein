function renderMenuTree(menus, container) {
    let html = '<table class="table table-bordered table-hover table-rounded">';
    html += '<thead class="thead-dark"><tr>';
    html += '<th style="width: 50%;"><input type="checkbox" id="checkAllMenus"> Menú</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllCrear"> Crear</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllLeer"> Leer</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllActualizar"> Actualizar</th>';
    html += '<th style="width: 12.5%; text-align: center;"><input type="checkbox" id="checkAllEliminar"> Eliminar</th>';
    html += '</tr></thead>';
    html += '<tbody>';

    menus.forEach(menu => {
        html += renderMenuItem(menu, 0);
    });

    html += '</tbody>';
    html += '</table>';
    container.html(html);

    // Eventos para expandir/colapsar
    container.on('click', '.toggle-submenu', function () {
        const $row = $(this).closest('tr');
        const secMenu = $row.data('sec-menu');
        $('tr[data-parent-menu="' + secMenu + '"]').toggle(200);
        $(this).find('i').toggleClass('fa-chevron-right fa-chevron-down');
    });

    // Evento para marcar/desmarcar todos los menús
    $('#checkAllMenus').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('.menu-checkbox').prop('checked', isChecked).trigger('change'); // Trigger change para propagar
    });

    // Eventos para marcar/desmarcar todos los permisos CRUD por columna
    $('#checkAllCrear').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Crear"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllLeer').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Leer"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllActualizar').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Actualizar"]').prop('checked', isChecked).trigger('change');
    });
    $('#checkAllEliminar').on('change', function () {
        const isChecked = $(this).prop('checked');
        $('input.permiso-checkbox[value="Eliminar"]').prop('checked', isChecked).trigger('change');
    });

    // Eventos para checkboxes de fila con independencia total entre Visibilidad y CRUD
    container.on('change', '.menu-checkbox', function () {
        const isChecked = $(this).prop('checked');
        const $row = $(this).closest('tr');
        const secMenu = $row.data('sec-menu');

        // Marcar/desmarcar SOLO la visibilidad de los submenús
        $('tr[data-parent-menu="' + secMenu + '"]').each(function () {
            $(this).find('.menu-checkbox').prop('checked', isChecked).trigger('change');
        });

        // Propagar hacia arriba si se marca (solo visibilidad)
        if (isChecked) {
            let currentParentSecMenu = $row.data('parent-menu');
            while (currentParentSecMenu) {
                const $parentRow = $('tr[data-sec-menu="' + currentParentSecMenu + '"]');
                $parentRow.find('.menu-checkbox').prop('checked', true);
                currentParentSecMenu = $parentRow.data('parent-menu');
            }
        }
    });

    // El checkbox de permiso CRUD ahora es totalmente independiente y no afecta a otros en la misma fila.
    container.on('change', '.permiso-checkbox', function () {
        // No se requiere ninguna acción de propagación automática a nivel de fila.
    });
}

function renderMenuItem(menu, level) {
    const hasSubmenus = menu.subMenus && menu.subMenus.$values && menu.subMenus.$values.length > 0;
    const baseIndent = 20; // Indentación base por nivel
    const currentIndent = level * baseIndent; // Indentación total para el nivel actual

    let html = `<tr data-sec-menu="${menu.secMenu}" ${menu.secMenuPadre ? 'data-parent-menu="' + menu.secMenuPadre + '"' : ''} style="${menu.secMenuPadre ? 'display: none;' : ''}">`;

    // Celda del Menú
    html += '<td style="width: 50%;">';
    html += '<div class="d-flex align-items-center" style="padding-left: ' + currentIndent + 'px;">';
    if (hasSubmenus) {
        html += '<span class="toggle-submenu mr-2" style="cursor: pointer;"><i class="fas fa-chevron-right"></i></span>';
    } else {
        html += '<span class="mr-2" style="width: 16px;"></span>'; // Espacio para alinear
    }
    html += `<input type="checkbox" class="menu-checkbox mr-2" value="${menu.secMenu}" ${menu.verMenu ? 'checked' : ''} data-sec-menu="${menu.secMenu}">`;
    html += `<i class="${menu.icono} mr-2"></i>`;
    html += `<span class="font-weight-bold">${menu.descripcion}</span>`;
    html += '</div>';
    html += '</td>';

    // Celdas de Permisos CRUD (solo si tiene un padre)
    const crudPermisosHtml = menu.secMenuPadre !== null ?
        `
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Crear" ${menu.crear ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Leer" ${menu.leer ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Actualizar" ${menu.actualizar ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        <td style="width: 12.5%; text-align: center;"><input type="checkbox" class="permiso-checkbox" value="Eliminar" ${menu.eliminar ? 'checked' : ''} data-sec-menu="${menu.secMenu}"></td>
        ` : `
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        <td style="width: 12.5%;"></td>
        `;
    html += crudPermisosHtml;

    html += '</tr>';

    // Submenús (se renderizan como filas adicionales, no anidadas en <ul>) - esto se manejará con data-parent-menu
    const subMenus = menu.subMenus && menu.subMenus.$values ? menu.subMenus.$values : menu.subMenus;
    if (subMenus && subMenus.length > 0) {
        subMenus.forEach(subMenu => {
            html += renderMenuItem(subMenu, level + 1);
        });
    }

    return html;
}

function collectPermissions(secRol, nombreRol) {
    const menuMap = new Map();
    const rootMenus = [];

    // 1. Leer el estado actual de todos los checkboxes del DOM y guardarlo en un mapa.
    $('#menuTreeContainer tbody tr').each(function () {
        const $row = $(this);
        const secMenu = parseInt($row.data('sec-menu'));
        const parentSecMenu = $row.data('parent-menu') ? parseInt($row.data('parent-menu')) : null;

        const menuData = {
            SecMenu: secMenu,
            SecMenuPadre: parentSecMenu,
            VerMenu: $row.find('.menu-checkbox').prop('checked'),
            Crear: $row.find('.permiso-checkbox[value="Crear"]').prop('checked'),
            Leer: $row.find('.permiso-checkbox[value="Leer"]').prop('checked'),
            Actualizar: $row.find('.permiso-checkbox[value="Actualizar"]').prop('checked'),
            Eliminar: $row.find('.permiso-checkbox[value="Eliminar"]').prop('checked'),
            SubMenus: []
        };
        menuMap.set(secMenu, menuData);
    });

    // 2. Reconstruir la jerarquía de menús usando el mapa.
    menuMap.forEach((menu, secMenu) => {
        if (menu.SecMenuPadre && menuMap.has(menu.SecMenuPadre)) {
            // Es un submenú, lo añadimos a su padre.
            menuMap.get(menu.SecMenuPadre).SubMenus.push(menu);
        } else {
            // Es un menú de nivel raíz.
            rootMenus.push(menu);
        }
    });

    return {
        SecRol: secRol,
        NombreRol: nombreRol,
        Menus: rootMenus
    };
}