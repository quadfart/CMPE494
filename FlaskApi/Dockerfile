FROM python:3.9-slim

ADD . /
WORKDIR /

RUN pip install --upgrade pip && \
    pip install flask tensorflow numpy pandas pillow

CMD ["python", "app/flask_api.py"]