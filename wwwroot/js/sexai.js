const entities = {
  students: { label: 'Студенты', fields: [['firstName','Имя или инициалы','text',true],['lastName','Фамилия','text',true],['age','Возраст','number',true],['course','Курс','number',true],['groupId','Группа','select',true]] },
  groups: { label: 'Группы', fields: [['name','Название группы','text',true],['direction','Направление','text',true]] },
  teachers: { label: 'Преподаватели', fields: [['firstName','Имя или инициалы','text',true],['lastName','Фамилия','text',true],['email','Email','email',false],['subjectId','Дисциплина','select',true]] },
  subjects: { label: 'Дисциплины', fields: [['name','Название дисциплины','text',true],['credits','Количество кредитов','number',true]] },
  grades: { label: 'Оценки', fields: [['studentId','Студент','select',true],['subjectId','Дисциплина','select',true],['teacherId','Преподаватель','select',true],['value','Оценка','number',true],['date','Дата','date',false]] }
};
let current = 'students'; const cache = {};
const tabs = document.querySelector('#tabs'), form = document.querySelector('#entity-form'), list = document.querySelector('#entity-list'), message = document.querySelector('#form-message');

for (const [key, entity] of Object.entries(entities)) { const button = document.createElement('button'); button.className = 'tab'; button.textContent = entity.label; button.addEventListener('click', () => selectEntity(key)); tabs.append(button); }

function selectEntity(key) {
  current = key; document.querySelectorAll('.tab').forEach((tab, index) => tab.classList.toggle('active', Object.keys(entities)[index] === key));
  document.querySelector('#form-title').textContent = `Добавить: ${entities[key].label.toLowerCase()}`; document.querySelector('#list-title').textContent = entities[key].label;
  message.textContent = ''; message.className = 'message'; renderForm(); loadItems();
}
function renderForm() {
  form.innerHTML = '';
  for (const [name, label, type, required] of entities[current].fields) {
    const labelElement = document.createElement('label'); labelElement.textContent = label; const control = type === 'select' ? document.createElement('select') : document.createElement('input');
    control.name = name; control.required = required; control.type = type === 'select' ? undefined : type; if (type === 'number') control.min = name === 'course' ? '1' : '0'; labelElement.append(control); form.append(labelElement);
  }
  const submit = document.createElement('button'); submit.className = 'primary'; submit.type = 'submit'; submit.textContent = 'Сохранить в базу'; form.append(submit); fillSelects();
}
async function api(path, options = {}) {
  const response = await fetch(path, { headers: { 'Content-Type': 'application/json' }, ...options }); const text = await response.text(); let body;
  try { body = text ? JSON.parse(text) : null; } catch { body = text; } if (!response.ok) throw new Error(body?.message || body?.title || `HTTP ${response.status}. Проверьте CollegeDB.`); return body;
}
async function loadItems() { list.innerHTML = '<div class="hint">Загрузка...</div>'; try { cache[current] = await api(`/api/${current}`); renderList(cache[current]); await fillSelects(); } catch (error) { list.innerHTML = `<div class="error">${error.message}</div>`; } }
function renderList(items) { if (!items?.length) { list.innerHTML = '<div class="empty">Записей пока нет.</div>'; return; } list.innerHTML = items.map(item => `<article class="card"><strong>${escapeHtml(displayName(current, item))}</strong><br><span>${escapeHtml(details(current, item))}</span></article>`).join(''); }
function displayName(type, item) { if (type === 'students' || type === 'teachers') return `${item.firstName} ${item.lastName}`; if (type === 'grades') return `Оценка: ${item.value}`; return item.name || `Запись №${item.id}`; }
function details(type, item) { if (type === 'students') return `Курс: ${item.course}, возраст: ${item.age}, группа: ${item.group?.name || item.groupId}`; if (type === 'teachers') return `${item.email || 'Email не указан'}; дисциплина: ${item.subject?.name || item.subjectId}`; if (type === 'groups') return `Направление: ${item.direction || 'не указано'}; студентов: ${item.students?.length || 0}`; if (type === 'subjects') return `Кредитов: ${item.credits}`; if (type === 'grades') return `${item.student ? `${item.student.firstName} ${item.student.lastName}` : `Студент №${item.studentId}`}; ${item.subject?.name || `предмет №${item.subjectId}`}`; return ''; }
async function fillSelects() {
  const sources = { groupId: 'groups', subjectId: 'subjects', studentId: 'students', teacherId: 'teachers' };
  for (const select of form.querySelectorAll('select')) { const source = sources[select.name]; if (!source) continue; try { const items = cache[source] || await api(`/api/${source}`); cache[source] = items; select.innerHTML = '<option value="">Выберите...</option>' + items.map(item => `<option value="${item.id}">${escapeHtml(displayName(source, item))}</option>`).join(''); } catch { select.innerHTML = '<option value="">Нет данных</option>'; } }
}
form.addEventListener('submit', async event => {
  event.preventDefault(); message.className = 'message'; message.textContent = 'Сохранение...'; const data = Object.fromEntries(new FormData(form).entries());
  for (const key of Object.keys(data)) if (['age','course','groupId','subjectId','studentId','teacherId','credits','value'].includes(key)) data[key] = Number(data[key]); if (!data.date && current === 'grades') data.date = new Date().toISOString();
  try { await api(`/api/${current}`, { method:'POST', body:JSON.stringify(data) }); form.reset(); message.textContent = 'Запись добавлена в базу.'; await loadItems(); } catch (error) { message.className = 'message error'; message.textContent = error.message; }
});
function escapeHtml(value) { return String(value ?? '').replace(/[&<>"']/g, char => ({ '&':'&amp;', '<':'&lt;', '>':'&gt;', '"':'&quot;', "'":'&#039;' }[char])); }
selectEntity('students');
